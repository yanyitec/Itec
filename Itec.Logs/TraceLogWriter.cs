using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Itec.Logs
{
    public class TraceLogWriter :FileLogWriter
    {
        public TraceLogWriter(string baseTraceLogDirectory = null) :base(
            baseTraceLogDirectory == null
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "__traces__")
            : baseTraceLogDirectory
            ) { 
        }

        string LastDir;
        protected override string GetFilename(LogEntry entry)
        {
            var logTime = entry.LogTime;
            var dirName = Path.Combine(this.BaseDirectory, entry.Category);
            if ((logTime - this.LastFiletime).Minutes >= 10|| LastDir==null)
            {
                if (logTime.Year != this.LastFiletime.Year || logTime.Month != this.LastFiletime.Month || logTime.Day != this.LastFiletime.Day
                    || logTime.Hour != this.LastFiletime.Hour || logTime.Minute != this.LastFiletime.Minute)
                {

                    dirName = LastDir = Path.Combine(dirName, logTime.ToString("yyyyMMdd/hhmm"));
                    EnsureDirExists(dirName);
                    return LastDir = dirName + "/" + entry.TraceId + ".txt";
                }
                
            }
            return Path.Combine(LastDir, entry.TraceId + ".txt");
        }

    }
}
