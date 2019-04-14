using System;

namespace Itec.Logs
{
    /// <summary>
    /// 写入数据用的日志实体
    /// </summary>
    public class LogEntity
    {
        public long Id { get; set; }

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
        /// 会话Id,
        /// 一般对应着UserId或sessionId
        /// </summary>
        public string SessionId { get; set; }
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
        public int Level { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 详细信息
        /// </summary>
        public string Details { get; set; }
    }
}
