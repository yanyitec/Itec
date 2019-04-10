using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Itec.Logs
{
    public class ConsoleLogWriter:LogWriter
    {
        public static Dictionary<LogLevels, ConsoleFormat> Formats = new Dictionary<LogLevels, ConsoleFormat>() {
            { LogLevels.All, new ConsoleFormat(ConsoleColor.White,"All","    ") }
            ,{ LogLevels.Debug, new ConsoleFormat(ConsoleColor.Blue,"Debug","  ") }
            ,{ LogLevels.Success, new ConsoleFormat(ConsoleColor.Green,"Success","") }
            ,{ LogLevels.Info,new ConsoleFormat( ConsoleColor.White,"Info","   ") }
            ,{ LogLevels.Notice, new ConsoleFormat(ConsoleColor.Yellow,"Notice"," ") }
            ,{ LogLevels.Warn, new ConsoleFormat(ConsoleColor.Magenta,"Warn","   ") }
            ,{ LogLevels.Error,new ConsoleFormat( ConsoleColor.Red,"Error","  ")}
        };

        public static ConsoleLogWriter Default = new ConsoleLogWriter();
        public struct ConsoleFormat
        {
            public ConsoleFormat(ConsoleColor color, string name, string space)
            {
                this.Color = color;
                this.LevelName = name;
                this.Space = space;
            }
            public ConsoleColor Color;
            public string LevelName;
            public string Space;
        }

        public override bool Equals(ILogWriter other)
        {
            return other != null && typeof(ConsoleLogWriter).IsAssignableFrom(other.GetType());
        }

        public static Encoding GB2312 = CodePagesEncodingProvider.Instance.GetEncoding("gb2312");
        protected override async Task WriteLog(LogEntry entry)
        {
            var fmt = Formats[entry.LogLevel];
            //var encoding = Console.OutputEncoding;
            //var color = Console.ForegroundColor;

            Console.OutputEncoding = GB2312;
            Console.ForegroundColor = fmt.Color;

            Console.Write("[");
            Console.Write(fmt.LevelName);
            Console.Write("]:");
            Console.Write(fmt.Space);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write('<');
            Console.Write(entry.LogTime.ToString("yyyy-MM-dd hh:ss:mm"));
            Console.Write("> ");
            if (entry.Category != null)
            {
                Console.Write(entry.Category);
            }

            if (entry.Host != null)
            {
                
                Console.Write("@");
                Console.Write(entry.Host);
                
            }

            if (entry.TraceId != null)
            {
                Console.Write(" {");
                Console.WriteLine(entry.TraceId);
                Console.Write(" }");
            }

            Console.WriteLine();


            Console.ForegroundColor = ConsoleColor.White;
            if (entry.Message != null)
            {
                var message = entry.MessageReplacements == null ? entry.Message : string.Format(entry.Message, entry.MessageReplacements);
                Console.WriteLine(message);
            }

            if (entry.DetailsObject != null)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[DETAILS]: ");
                //Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(this.Formater == null ? entry.DetailsObject.ToString() : this.Formater.Format(entry.DetailsObject));
            }
            Console.WriteLine();


            //Console.OutputEncoding = encoding;
            //Console.ForegroundColor = color;

        }

    }
}
