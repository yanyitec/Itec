using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Logs
{
    /// <summary>
    /// 日志写入链节点
    /// </summary>
    public sealed class WritingClainNode
    {
        public WritingClainNode(LogEntry entry) {
            this.Entry = entry;
        }
        public WritingClainNode Next { get; internal set; }
        public LogEntry Entry { get; private set; }
    }
}
