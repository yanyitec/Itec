using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Logs
{
    public interface ILogWriter: IEnumerable<ILogWriter>,IEquatable<ILogWriter>
    {
        void RecordLog(LogEntry entry);

        Task PersistentLogs(WritingClainNode node);

        ILogWriter AddLogWriter(ILogWriter writer);

        bool IsCollection { get; }

        int Count { get; }

        IDetailsFormater Formater { get; set; }

        ILogWriter Clone(string category);
    }
}
