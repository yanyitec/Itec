using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Logs
{
    public class Logger : ILogger
    {
        public static Logger Default = new Logger(Logs.LoggerFactory.Default,System.Net.Dns.GetHostName(),LogWriter.Default,"",null,null);
        public Logger(ILoggerFactory loggerFactory, string host,ILogWriter categoryWriter, string category,ILogWriter traceWriter, string traceId) {
            this.Host = host;
            this.CategoryWriter = categoryWriter;
            if(traceWriter!=null)this.TraceWriter = traceWriter.AddLogWriter(categoryWriter);
            this.TraceId = traceId;
            this.LoggerFactory = loggerFactory;
            this.Category = category;

        }
        public ILoggerFactory LoggerFactory { get; private set; }
        public string Host { get; private set; }
        public string Category {
            get;
            private set;
        }

        public string TraceId { get; private set; }

        public ILogWriter TraceWriter { get; private set; }
        public ILogWriter CategoryWriter { get; private set; }

        public void Log(LogLevels lv ,object details, string message, params object[] args)
        {
            if (this.TraceWriter == null
                && (int)this.LoggerFactory.LogLevel > (int)lv
                ) return;

            var entry = new LogEntry()
            {
                LogLevel = lv,
                Message = message,
                DetailsObject = details,
                TraceId = TraceId,
                Category = Category,
                Host = Host,
                MessageReplacements = args
            };

            if (this.TraceWriter != null && this.TraceId != null)
                this.TraceWriter.RecordLog(entry);
            else if((int)this.LoggerFactory.LogLevel <= (int)lv)
                this.CategoryWriter.RecordLog(entry);
        }


        public void MessageDetails(object details,string message, params object[] args) {
            this.Log(LogLevels.All, details, message, args);
        }
        public void Message( string message, params object[] args)
        {
            this.Log(LogLevels.All,null,message,args);
        }
        public void DebugDetails(object details, string message,params object[] args)
        {
            this.Log(LogLevels.Debug, details, message, args);
        }

        public void Debug(string message, params object[] args)
        {
            this.Log(LogLevels.Debug, null, message, args);
        }
        public void SuccessDetails(object details, string message,params object[] args)
        {
            this.Log(LogLevels.Success, details, message, args);
        }

        public void Success( string message, params object[] args)
        {
            this.Log(LogLevels.Success, null, message, args);
        }

        public void InfoDetails(object details, string message,params object[] args)
        {

            this.Log(LogLevels.Info, details, message, args);
        }

        public void Info( string message, params object[] args)
        {
            this.Log(LogLevels.Info, null, message, args);
        }

        public void NoticeDetails(object details, string message,params object[] args)
        {
            this.Log(LogLevels.Notice, details, message, args);
        }

        public void Notice(string message, params object[] args)
        {
            this.Log(LogLevels.Notice, null, message, args);
        }

        public void WarnDetails(object details, string message,params object[] args)
        {
            this.Log(LogLevels.Warn, details, message, args);
        }
        public void Warn(string message, params object[] args)
        {
            this.Log(LogLevels.Warn, null, message, args);
        }

        public void ErrorDetails(object details, string message,params object[] args)
        {
            this.Log(LogLevels.Error, details, message, args);
        }
        public void Error(string message, params object[] args)
        {
            this.Log(LogLevels.Error, null, message, args);
        }

        public void Error(Exception details, string message = null, params object[] args) {
            this.Log(LogLevels.Error, details, message==null?details?.Message:message, args);
        }
    }
}
