using System;

namespace Itec.Logs
{
    public interface ILogger
    {
        ILoggerFactory LoggerFactory { get; }
        string Category { get; }
        ILogWriter CategoryWriter { get; }
        ILogWriter TraceWriter { get;  }
        void Log(LogLevels lv,object details,string message,params object[] args);
        void MessageDetails(object details, string message, params object[] args);
        void Message(string message, params object[] args);

        void DebugDetails(object details, string message, params object[] args);
        void Debug(string message, params object[] args);
        void SuccessDetails(object details, string message, params object[] args);
        void Success(string message, params object[] args);
        void InfoDetails(object details, string message, params object[] args);
        void Info(string message, params object[] args);
        void NoticeDetails(object details, string message, params object[] args);
        void Notice(string message, params object[] args);
        void WarnDetails(object details, string message, params object[] args);
        void Warn(string message, params object[] args);
        void ErrorDetails(object details, string message, params object[] args);
        void Error(string message, params object[] args);

        void Error(Exception ex, string message=null, params object[] args);
        
        
        
        
        
    }
}