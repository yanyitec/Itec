using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Logs
{
    public class FileLogWriter:LogWriter
    {
        public new static FileLogWriter Default = new FileLogWriter();
        public FileLogWriter(string baseLogDirectory=null) {
            if (baseLogDirectory == null) {
                baseLogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            }
            this.BaseDirectory = baseLogDirectory;
            
        }

        public string BaseDirectory { get; private set; }

        public override bool Equals(ILogWriter obj)
        {
            var other = obj as FileLogWriter;
            if (other == null) return false;
            return other.BaseDirectory == this.BaseDirectory;
        }




        protected string LastFilename;
        protected string LastDir;
        protected DateTime LastFiletime;

        public static void EnsureDirExists(string path,bool isfile=false) {
            if (!isfile) {
                if (!Directory.Exists(path)) {
                    try {
                        Directory.CreateDirectory(path);
                    } catch { }
                }
            }
            var logDirs = path.Replace("\\", "/").Split("/");
            path = logDirs[0];

            for (var i = 1; i < (isfile?logDirs.Length - 1:logDirs.Length); i++)
            {
                path += "/" + logDirs[i];
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
        }


        protected virtual string GetFilename(LogEntry entry) {
            var logTime = entry.LogTime;
            
            
            if ((logTime - this.LastFiletime).Minutes > 10)
            {
                if (logTime.Year != this.LastFiletime.Year || logTime.Month != this.LastFiletime.Month || logTime.Day != this.LastFiletime.Day || logTime.Hour != this.LastFiletime.Hour) {
                    var dir = Path.Combine(this.BaseDirectory, logTime.ToString("yyyyMMdd"));
                    EnsureDirExists(dir);
                    return dir + "/" + logTime.ToString("hhmm") + ".txt";
                }
                return Path.Combine(this.BaseDirectory, logTime.ToString("yyyyMMdd/hhmm.txt"));
            }
            else return this.LastFilename;
            
        }

        void GetFileStream(LogEntry entry,ref TextWriter stream) {
            var filename = this.GetFilename(entry);
            if (filename == this.LastFilename) return;
            if(stream!=null)stream.Dispose();
            try {
                stream = File.AppendText(filename);
            } catch {
                EnsureDirExists(filename, true);
                stream = File.AppendText(filename);

            }
            this.LastFilename = filename;
            this.LastFiletime = entry.LogTime;
        }
        public static string Spliter = "----------------------------------------";
        async Task WriteToStream(LogEntry entry, TextWriter stream) {
            var fmt = ConsoleLogWriter.Formats[entry.LogLevel];
            await stream.WriteLineAsync(Spliter);
            await stream.WriteAsync("[");
            await stream.WriteAsync(fmt.LevelName);
            await stream.WriteAsync("]");
            await stream.WriteAsync(fmt.Space);
            await stream.WriteAsync('<');
            await stream.WriteAsync(entry.LogTime.ToString("yyyy-MM-dd hh:mm:ss"));
            await stream.WriteAsync("> ");
            if (entry.Category != null)
            {
                await stream.WriteAsync(entry.Category);
            }
            if (entry.Host != null)
            {
                await stream.WriteAsync("@");
                await stream.WriteAsync(entry.Host);
            }

            if (entry.TraceId != null)
            {

                await stream.WriteAsync(" {");
                await stream.WriteAsync(entry.TraceId);
                await stream.WriteAsync("}");
            }
            await stream.WriteLineAsync();
            if (entry.Message != null) {
                var message = entry.MessageReplacements == null ? entry.Message : string.Format(entry.Message, entry.MessageReplacements);
                await stream.WriteLineAsync(message);
            }
            
            
            

            if (entry.DetailsObject != null)
            {
                await stream.WriteLineAsync("[DETAILS]:");
                string msg = null;
                try {
                    msg = this.Formater == null ? entry.DetailsObject.ToString() : this.Formater.Format(entry.DetailsObject);
                } catch { }
                await stream.WriteLineAsync(msg);
            }
            await stream.WriteLineAsync();
        }
        
        public override async Task PersistentLogs(WritingClainNode node)
        {
            TextWriter stream = null;
            while (node != null) {
                try {
                    var entry = node.Entry;
                    GetFileStream(entry, ref stream);
                    await WriteToStream(entry,stream);
                } catch(Exception ex) {
                    var c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ForegroundColor = c;
                }
                node = node.Next;
                
            }
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        public override ILogWriter Clone(string category)
        {
            return new FileLogWriter(Path.Combine(this.BaseDirectory,string.IsNullOrEmpty(category)?"__default__":category));
        }


    }
}
