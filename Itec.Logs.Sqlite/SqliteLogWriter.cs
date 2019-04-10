using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Logs
{
    public class SqliteLogWriter:DbLogWriter
    {
        public SqliteLogWriter(string logDbName=null) : base(logDbName) {
            if (logDbName == null) {
                logDbName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/local.db");
            }
            ConnectionString = "Data Source=" + logDbName;
        }
        public string ConnectionString { get; private set; }
        protected override DbConnection GetOrCreateConnection(string connName)
        {
            return new SQLiteConnection(this.ConnectionString);
        }

        public async Task PersistentLogsxx(WritingClainNode node)
        {
            using (var conn = this.GetOrCreateConnection(this.DbName))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(DbLogWriter.CreateTableSql, "itec_");
                cmd.ExecuteNonQuery();
            }

        }
    }
}
