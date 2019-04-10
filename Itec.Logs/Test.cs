using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itec.Logs
{
    public static class Test
    {
        public static void Main(string[] args) {
            Console.OutputEncoding = ConsoleLogWriter.GB2312;
            Console.WriteLine("====基本测试===");
            UseLog(Logger.Default);
            Console.WriteLine("====ConsoleLoggerWriter不会添加两次===");
            LoggerFactory.Default.AddCategoryWriter(ConsoleLogWriter.Default);
            Console.WriteLine(".AddCategoryWriter(ConsoleLogWriter.Default):"+ LoggerFactory.Default.CategoryLogWriter.Count.ToString());
            Console.WriteLine("====添加默认的文件日志===");
            LoggerFactory.Default.AddCategoryWriter(FileLogWriter.Default);
            Console.WriteLine("====只要目录名一样，就认为是相同的LogWriter，不会重复添加===");
            var otherFileLogger = new FileLogWriter();
            LoggerFactory.Default.AddCategoryWriter(otherFileLogger);
            Console.WriteLine(".AddCategoryWriter(otherFileLogger):" + LoggerFactory.Default.CategoryLogWriter.Count.ToString());
            Console.WriteLine("====如果相同，新的LogWriter会替换旧的===");
            Console.WriteLine("LoggerFactory.Default.CategoryLogWriter.Last()==otherFileLogger:"
                + (LoggerFactory.Default.CategoryLogWriter.Last()==otherFileLogger).ToString());

            Console.WriteLine("====从LoggerFactory中获取一个Category=Auth的Logger===");
            var authLogger = LoggerFactory.Default.GetOrCreateLogger("Auth");
            Console.WriteLine("authLogger.Category=" + authLogger.Category);
            Console.WriteLine("====向authLogger写一轮日志===");
            UseLog(authLogger);

            Console.WriteLine("Press any to exit...");
            Console.ReadKey();
        }

        public static void UseLog(ILogger logger) {
            logger.Debug("我是{0}","Debug");
            logger.Success("我是{0}", "Success");
            logger.Info("我是{0}", "Info");
            logger.Notice("我是{0}", "Notice");
            logger.Warn("我是{0}", "Warn");
            logger.Error("我是{0}", "Error");
        }
    }
}
