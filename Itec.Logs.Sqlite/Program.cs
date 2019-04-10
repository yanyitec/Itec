using System;
using System.Data.SQLite;
using System.IO;

namespace Itec.Logs.Sqlite
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/local.db");
            FileLogWriter.EnsureDirExists(dbName,true);
            using (var conn = new SQLiteConnection("Data Source="+dbName)) {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(DbLogWriter.CreateTableSql, "itec_");
                //cmd.ExecuteNonQuery();
            }
            Itec.Logs.LoggerFactory.Default.AddCategoryWriter(new SqliteLogWriter());
            var logger = Itec.Logs.LoggerFactory.Default.GetOrCreateLogger("sqlite");
            Test.UseLog(logger);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


    }
}
