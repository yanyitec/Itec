using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Logs
{
    public class LogWriterCollection : ILogWriter
    {
        Queue<ILogWriter> _Writers;
        public LogWriterCollection()
        {
            this._Writers = new Queue<ILogWriter>();
        }

        public LogWriterCollection(IEnumerable<ILogWriter> writers) : this() {
            foreach (var w in writers) this.AddLogWriter(w);
        }
        public LogWriterCollection(params ILogWriter[] writers) : this()
        {
            foreach (var w in writers) this.AddLogWriter(w);
        }
        public bool IsCollection => true;

        public IDetailsFormater Formater {
            get { return this._Writers.FirstOrDefault()?.Formater; }
            set { foreach (var w in this._Writers) w.Formater = value; }
        }

        public void RecordLog(LogEntry entry)
        {
            foreach (var writer in _Writers) {
                writer.RecordLog(entry);
            }
        }

        public ILogWriter AddLogWriter(ILogWriter writer) {
            if (writer == null) return this;
            if (writer.IsCollection)
            {
                
                foreach (var toAdd in writer)
                {
                    var replaced = false;
                    for (int i = 0, j = this._Writers.Count; i < j; i++) {
                        var existed = this._Writers.Dequeue();
                        if (!replaced && existed.Equals(toAdd))
                        {
                            this._Writers.Enqueue(toAdd);
                            replaced = true;
                        }
                        else {
                            this._Writers.Enqueue(existed);
                        } 
                    }
                    if (!replaced) _Writers.Enqueue(toAdd);

                }
            }
            else {
                bool added = false;
                for (int i = 0, j = this._Writers.Count; i < j; i++)
                {
                    var existed = this._Writers.Dequeue();
                    if (existed.Equals(writer))
                    {
                        this._Writers.Enqueue(writer);
                        added = true;
                    }
                    else
                    {
                        this._Writers.Enqueue(existed);
                    }
                }
                if (!added) this._Writers.Enqueue(writer);
            }
            
            return this;

        }

        public Task PersistentLogs(WritingClainNode node)
        {
            throw new InvalidOperationException("LogWriterCollection不应该调用该方法，调用该方法前应该检查IsCollection属性");
        }

        public ILogWriter Clone(string category) {
            var ls = new List<ILogWriter>();
            foreach (var w in this._Writers) ls.Add(w.Clone(category));
            return new LogWriterCollection(ls);
        }

        public IEnumerator<ILogWriter> GetEnumerator()
        {
            return this._Writers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Writers.GetEnumerator();
        }

        public int Count { get { return this._Writers.Count; } }

        public bool Equals(ILogWriter other)
        {
            if (other == null) return false;
            if (other.IsCollection) {
                if (other.Count != this.Count) return false;
                foreach (var w in this._Writers) {
                    if (!other.Any(p => w.Equals(p))) {
                        return false;
                    }
                    
                }
                return true;
            } else {
                return this._Writers.Count == 1 && this._Writers.First() == other;
            }
        }
    }
}
