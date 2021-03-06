﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewLife;
using NewLife.Log;
using Stardust.Models;

namespace Stardust
{
    /// <summary>应用服务管理</summary>
    public class ServiceManager : DisposeBase
    {
        #region 属性
        public ServiceInfo[] Services { get; set; }

        private readonly List<Process> _processes = new List<Process>();
        #endregion

        #region 构造
        protected override void Dispose(Boolean disposing)
        {
            base.Dispose(disposing);

            Stop(disposing ? "Dispose" : "GC");
        }
        #endregion

        #region 方法
        public void Start()
        {
            var ts = new List<Task<Process>>();
            foreach (var item in Services)
            {
                if (item.AutoStart) ts.Add(Task.Run(() => StartService(item)));
            }

            // 等待全部完成
            var ps = Task.WhenAll(ts).Result;
            _processes.AddRange(ps.Where(e => e != null));
        }

        private Process StartService(ServiceInfo service)
        {
            WriteLog("启动应用[{0}]：{1} {2}", service.Name, service.FileName, service.Arguments);

            // 修正路径
            var workDir = "";
            var file = service.FileName;
            if (file.Contains("/") || file.Contains("\\"))
            {
                file = file.GetFullPath();
                workDir = Path.GetDirectoryName(file);
            }

            var si = new ProcessStartInfo
            {
                FileName = file,
                Arguments = service.Arguments,
                WorkingDirectory = workDir,
            };

            var retry = service.Retry;
            if (retry <= 0) retry = 1024;
            for (var i = 0; i < retry; i++)
            {
                try
                {
                    var p = Process.Start(si);

                    WriteLog("应用[{0}]启动成功 PID={1}", service.Name, p.Id);

                    return p;
                }
                catch (Exception ex)
                {
                    Log?.Write(LogLevel.Error, "{0}", ex);

                    Thread.Sleep(5_000);
                }
            }

            return null;
        }

        public void Stop(String reason)
        {
            foreach (var item in _processes)
            {
                WriteLog("停止应用[{0}] PID={1} {2}", item.ProcessName, item.Id, reason);

                item.Kill();
            }
            _processes.Clear();
        }
        #endregion

        #region 日志
        public ILog Log { get; set; }

        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
        #endregion
    }
}