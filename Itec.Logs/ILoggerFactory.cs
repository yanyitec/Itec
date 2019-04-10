namespace Itec.Logs
{
    public interface ILoggerFactory
    {
        ILogWriter CategoryLogWriter { get; set; }
        IDetailsFormater DetailsFormater { get; }

        LogLevels LogLevel { get; set; }
        string Host { get; }
        ILogWriter TraceLogWriter { get; set; }

        ILoggerFactory AddCategoryWriter(ILogWriter writer);
        ILoggerFactory AddTraceWriter(ILogWriter writer);
        ILogger GetOrCreateLogger(string category, string traceId = null);
    }
}