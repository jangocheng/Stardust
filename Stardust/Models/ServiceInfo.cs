﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stardust.Models
{
    /// <summary>服务信息</summary>
    public class ServiceInfo
    {
        #region 属性
        /// <summary>名称</summary>
        [XmlAttribute]
        public String Name { get; set; }

        /// <summary>文件</summary>
        [XmlAttribute]
        public String FileName { get; set; }

        /// <summary>参数</summary>
        [XmlAttribute]
        public String Arguments { get; set; }

        ///// <summary>工作目录</summary>
        //[XmlAttribute]
        //public String WorkingDirectory { get; set; }

        /// <summary>是否自动启动</summary>
        [XmlAttribute]
        public Boolean AutoStart { get; set; }

        /// <summary>启动失败时的重试次数，默认3次</summary>
        [XmlAttribute]
        public Int32 Retry { get; set; } = 3;

        /// <summary>是否自动重启</summary>
        [XmlAttribute]
        public Boolean AutoRestart { get; set; }

        /// <summary>重启退出代码。仅有该退出代码才会重启</summary>
        [XmlAttribute]
        public String RestartExistCodes { get; set; }
        #endregion
    }
}