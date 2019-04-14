using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Logs
{
    /// <summary>
    /// 日志内存实体
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>
        /// 日志发生时间
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 跟踪名，
        /// 日志跟踪用，
        /// 一般对应着Url或Controller的类名
        /// </summary>
        public string TraceId { get; set; }
        
        /// <summary>
        /// 日志分类
        /// 一般对应着类名
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 日志产生的程序主机名
        /// 一般是local_ip:port的形式
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevels LogLevel { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 消息参数
        /// </summary>

        public object[] MessageReplacements { get; set; }
        /// <summary>
        /// 详情对象
        /// </summary>

        public object DetailsObject { get; set; }
    }
}
