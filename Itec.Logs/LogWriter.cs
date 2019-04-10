using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Logs
{
    public abstract class LogWriter : ILogWriter {
        public static LogWriter Default = ConsoleLogWriter.Default;
        public static LogWriter DefaultError = new FileLogWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Logs/__err__"));
        public static LogWriter DefaultTrace = new TraceLogWriter();

        public IDetailsFormater Formater { get; set; }
        public WritingClainNode Head { get; private set; }
        public WritingClainNode Tail { get; private set; }
        Task _WritingTask;
        public bool IsCollection => false;
        public int Count => 1;
        public void RecordLog(LogEntry entry) {
            entry.LogTime = DateTime.Now;
            var node = new WritingClainNode(entry);
            lock (this)
            {
                
                if (Head == null)
                {
                    Head = Tail = node;
                }
                else
                {

                    Tail.Next = node;
                    Tail = node;
                }
                if (_WritingTask == null)
                {
                    _WritingTask = Task.Run(this.WriteLogs);
                }
            }
        }


        async Task WriteLogs() {
            var totalKeepCount = 80;
            var keepCount = totalKeepCount;           
            var delayMilliseconds = 120;
            while (true) {
                
                WritingClainNode node = null;
                lock (this) {
                    node = this.Head;
                    this.Head = this.Tail = null;
                    
                }
                if (node == null)
                {
                    if (--keepCount <= 0)
                    {
                        lock (this) {
                            if(this.Head==null) this._WritingTask = null;
                        }
                        
                        return;
                    }
                } else {
                    keepCount = totalKeepCount;
                    await this.PersistentLogs(node);
                }
                await Task.Delay(delayMilliseconds);


            }
        }

        public virtual async Task PersistentLogs(WritingClainNode node) {
            while (node != null) {
                try {
                    await this.WriteLog(node.Entry);
                    node = node.Next;
                } catch (Exception ex) {
                    var c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ForegroundColor = c;
                }
                
            }
        }

        protected virtual Task WriteLog(LogEntry entry) {
            throw new InvalidOperationException("需要继承的Writer重写WriteLog/WriteLogs函数");
        }

        
        public virtual ILogWriter Clone(string category) { return this; }

        public ILogWriter AddLogWriter(ILogWriter writer)
        {

            if (writer == null) {
                return this;
            }
            if (this.Equals(writer))
            {
                return writer;
            }
            var rs = new LogWriterCollection();
            rs.AddLogWriter(this);
            rs.AddLogWriter(writer);
            return rs;


        }

        class InternalEnumerator : IEnumerator<ILogWriter>
        {
            public InternalEnumerator(LogWriter writer) {
                this.Current = writer;
            }


            public bool IsRemoved { get; private set; }
            public ILogWriter Current { get; private set; }

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (this.IsRemoved) return false;
                return this.IsRemoved = true;
                
            }

            public void Reset()
            {
                this.IsRemoved = false;
            }
        }

        public IEnumerator<ILogWriter> GetEnumerator()
        {
            return new InternalEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new InternalEnumerator(this);
        }

        public virtual bool Equals(ILogWriter other)
        {

            if (other.IsCollection)
            {
                return other.Count==0 && this == other.First();
            }
            else return this == other;
            
        }
    }
}
