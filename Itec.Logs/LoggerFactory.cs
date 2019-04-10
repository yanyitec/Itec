using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Itec.Logs
{
    public class LoggerFactory : ILoggerFactory
    {
        public static LoggerFactory Default = new LoggerFactory(System.Net.Dns.GetHostName(),null);

        public LoggerFactory(string host, IDetailsFormater format) {
            this.Host = host;
            this.DetailsFormater = format;
            this.CategoryLogWriter = this.TraceLogWriter = LogWriter.Default;
            this._CategoryWriters = new ConcurrentDictionary<string, ILogger>();
        }
        public string Host { get; private set; }

        public IDetailsFormater DetailsFormater { get; private set; }

        
        ConcurrentDictionary<string, ILogger> _CategoryWriters;
        
        public ILogWriter CategoryLogWriter { get; set; }
        public ILogWriter TraceLogWriter { get; set; }
        LogLevels _LogLevel;
        public LogLevels LogLevel {
            get {
                return _LogLevel;
            }
            set {
                lock (this) _LogLevel = value;
            }
        }

        public ILogger GetOrCreateLogger(string category,string logTraceId=null) {
            var logger = _CategoryWriters.GetOrAdd(category,(cate)=> new Logger(this,this.Host, CategoryLogWriter.Clone(cate), category, null, null));
            if (logTraceId == null) return logger;
            return new Logger(this,this.Host,logger.CategoryWriter, category, logger.TraceWriter, logTraceId);
        }

        public ILoggerFactory AddCategoryWriter(ILogWriter writer) {
            if (this.CategoryLogWriter == null)
            {
                this.CategoryLogWriter = writer;
                writer.Formater = this.DetailsFormater;
            }
            else {
                this.CategoryLogWriter = this.CategoryLogWriter.AddLogWriter(writer);
            }
            return this;
        }

        public ILoggerFactory AddTraceWriter(ILogWriter writer)
        {
            writer.Formater = this.DetailsFormater;
            if (this.TraceLogWriter == null)
            {
                this.TraceLogWriter = writer;
            }
            else
            {
                this.TraceLogWriter = this.TraceLogWriter.AddLogWriter(writer);
            }
            return this;

        }

    }
}
