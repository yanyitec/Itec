using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Logs
{
    public abstract class DbLogWriter : LogWriter
    {
        public DbLogWriter(string logDbName) {
            this.DbName = logDbName;
        }

        
        public string DbName { get; private set; }

        public override bool Equals(ILogWriter obj)
        {
            var other = obj as DbLogWriter;
            if (other == null) return false;
            return other.DbName == this.DbName;
        }

        public static string CreateTableSql = @"
CREATE TABLE {0}Logs (
    LogTime DateTime,
    Host varchar(256),
    Category varchar(256),
    TraceId varchar(512),
    LogLevel int,
    Message varchar(1024),
    Details text
);
";

        

        public override async Task PersistentLogs(WritingClainNode node)
        {
            using (var conn = this.GetOrCreateConnection(this.DbName)) {
                await conn.OpenAsync();
                while (node != null)
                {
                    var cmd = this.BuildCommand(conn,node.Entry);
                    try {
                        await cmd.ExecuteNonQueryAsync();
                    } catch(Exception ex) {
                        var c = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Console.ForegroundColor = c;
                    }
                    
                    node = node.Next;
                }
                this.Command = null;
            }
                
        }

        protected virtual string MakeSql() {
            var sql = @"
INSERT INTO itec_Logs (
    Category,Details,Host,LogLevel,LogTime,Message,TraceId
) VALUES(
    @Category,@Details,@Host,@LogLevel,@LogTime,@Message,@TraceId
)";
            return sql;
        }

        protected abstract DbConnection GetOrCreateConnection(string connName);
        protected DbCommand Command;
        protected virtual DbCommand BuildCommand( DbConnection conn, LogEntry entry) {
            if (Command != null)
            {
                //Command.Parameters.Clear();
                //AddParameters(Command, entry);
            }
            {
                Command = conn.CreateCommand();
                Command.CommandText = this.MakeSql();
                AddParameters(Command, entry);
            }
            return Command;
        }

        protected virtual bool ParameterNeedName{get;set;}

        void AddParameters(IDbCommand cmd, LogEntry entry) {
            var dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@Category";
            dbParam.DbType = DbType.String;
            dbParam.Value = entry.Category??string.Empty;
            cmd.Parameters.Add(dbParam);

            dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@Details";
            dbParam.DbType = DbType.String;
            dbParam.Value = entry.DetailsObject==null?string.Empty:(Formater==null?entry.DetailsObject.ToString():Formater.Format(entry.DetailsObject));
            cmd.Parameters.Add(dbParam);

            dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@Host";
            dbParam.DbType = DbType.String;
            dbParam.Value = entry.Host ?? string.Empty;
            cmd.Parameters.Add(dbParam);

            dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@LogLevel";
            dbParam.DbType = DbType.Int32;
            dbParam.Value = (int)entry.LogLevel;
            cmd.Parameters.Add(dbParam);

            dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@LogTime";
            dbParam.DbType = DbType.DateTime;
            dbParam.Value = entry.LogTime;
            cmd.Parameters.Add(dbParam);

            dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@Message";
            dbParam.DbType = DbType.String;
            dbParam.Value = entry.Message==null?string.Empty:(entry.MessageReplacements==null?entry.Message:string.Format(entry.Message,entry.MessageReplacements));
            cmd.Parameters.Add(dbParam);

            dbParam = cmd.CreateParameter();
            dbParam.ParameterName = "@TraceId";
            dbParam.DbType = DbType.String;
            dbParam.Value = entry.TraceId ?? string.Empty;
            cmd.Parameters.Add(dbParam);
        }
    }
}
