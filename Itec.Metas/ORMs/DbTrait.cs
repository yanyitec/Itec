
using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs
{
    /// <summary>                                                                
    /// 数据库特征类
    /// mysql/sqlserver等一个数据库一种特性
    /// </summary>
    public class DbTrait
    {
        public IDictionary<Guid, DbType> DataTypeMaps { get;protected set; }

        public SqlParametricKinds ParametricKind { get;protected set; }

        public virtual char NameStart { get; protected set; }
        public virtual char NameEnd { get; protected set; }

        

        


        public string SqlFieldname(string name) {
            var rs = string.Empty;
            if (this.NameStart != '\0') rs += this.NameStart;
            rs += name;
            if (this.NameEnd != '\0') rs += this.NameEnd;
            return rs;
        }

        public string SqlTablename(string name)
        {
            var rs = string.Empty;
            if (this.NameStart != '\0') rs += this.NameStart;
            rs += name;
            if (this.NameEnd != '\0') rs += this.NameEnd;
            return rs;
        }



        public virtual DbConnection CreateConnection(string connString) {
            return new System.Data.SQLite.SQLiteConnection(connString);
        }

        public virtual DbCommand CreateDbCommand(DbConnection conn, string cmdText) {
            return new System.Data.SQLite.SQLiteCommand(cmdText, conn as System.Data.SQLite.SQLiteConnection);
        }

        public string PrimaryKeyword { get; set; }

        

        public virtual string CheckTableExistsSql(string tbName) {
            
            return $"SELECT COUNT(*) FROM sqlite_master where type='table' and name='{tbName}'";
        }

        public virtual DbType GetDbType(string str) {
            var t = str.ToLower();
            if (t == "char") return DbType.AnsiStringFixedLength;
            if (t == "nchar") return DbType.StringFixedLength;
            if (t == "text" || t=="nvarchar") return DbType.String;
            if (t == "varchar" ) return DbType.AnsiString;
            if (t == "date") return DbType.Date;
            if (t == "datetime") return DbType.DateTime;
            if (t == "guid") return DbType.Guid;
            if (t == "bool") return DbType.Boolean;
            if (t == "bit") return DbType.Boolean;
            if (t == "decimal") return DbType.Decimal;
            if (t == "float"|| t=="double") return DbType.Double;
            var dbType = str.ConvertTo<DbType>();
            if (dbType.HasValue) return dbType.Value;
            return DbType.String;
        }
        public virtual DbField MakeField(IDataReader reader) {
            var name = reader.GetString(1);
            var type = reader.GetString(2);
            int length = 0;
            int precision = 0;

            var beginAt = type.IndexOf("(");
            if (beginAt > 0)
            {
                var endAt = type.IndexOf(")", beginAt);
                var c = type.Substring(beginAt + 1, endAt - beginAt - 2);
                var cs = c.Split(',');
                length = int.Parse(cs[0]);
                if (cs.Length == 2)
                {
                    precision = int.Parse(cs[1]);
                }
                type = type.Substring(0, beginAt - 1);
            }
            bool nullable = reader.GetInt32(3) != 1;
            bool isPri = reader.GetInt32(4) != 1;
            var field = new DbField(name, GetDbType(type), nullable, length, precision, isPri);
            return field;
        }
        public virtual string QueryFieldsSql(string tbName) {
            var sql= $"PRAGMA  table_info('{tbName}'）";
            return sql;
            
        }

        



        
       

        public virtual DbType GetDbType(MetaProperty prop) {
            DbType dbType = DbType.String;
            if (this.DataTypeMaps != null && this.DataTypeMaps.Count > 0) {
                this.DataTypeMaps.TryGetValue(prop.NonullableType.GUID,out dbType);
            }
            return dbType;
        }

        public virtual string GetSqlFieldType(IDbProperty prop) {
            var hash = prop.NonullableType.GetHashCode();
            if(typeof(byte).GetHashCode()==hash)return "TINYINT";
            if (typeof(short).GetHashCode() == hash) return "MEDIUMINT";
            if (typeof(ushort).GetHashCode() == hash) return "MEDIUMINT";
            if (typeof(int).GetHashCode() == hash) return "INT";
            if (typeof(uint).GetHashCode() == hash) return "INT";
            if (typeof(long).GetHashCode() == hash) return "BIGINT";
            if (typeof(ulong).GetHashCode() == hash) return "UNSIGNED BIG INT";
            if (typeof(float).GetHashCode() == hash) return "FLOAT";
            if (typeof(double).GetHashCode() == hash) return "DOUBLE";
            if (typeof(bool).GetHashCode() == hash) return "INT";
            if (typeof(decimal).GetHashCode() == hash) return "DECIMAL";
            if (typeof(Guid).GetHashCode() == hash) return "CHAR";
            if (typeof(DateTime).GetHashCode() == hash) return "DATETIME";
            return "TEXT";
        }
        public virtual string GetSqlPrecision(IDbProperty prop) {
            var field = prop.Field;
            if (field.DbType == DbType.Guid) return "64";
            if (field.DbType == DbType.String || field.DbType == DbType.StringFixedLength || field.DbType == DbType.AnsiString || field.DbType == DbType.AnsiStringFixedLength) {
                return (field.Length??256).ToString();
            }
            if (field.DbType == DbType.Currency || field.DbType == DbType.Decimal) {
                return (field.Length ?? 48).ToString() + "," + (field.Precision??8).ToString();
            }
            return null;
        }

               

        
      

        
        

        public static string SafeString(string str) {
            return str.Replace("'", "''");//.Replace("\n","\\n").Replace("\r", "\\r");
        }

        public static string SqlValue(object value,bool nullable=false,object defaultValue=null) {
            if (value == null) {
                if (nullable) return "NULL";
                else value = defaultValue??"";
            }
            var t = value.GetType();
            if (t == typeof(DateTime)) return "'1790-1-1'";
            if (t.IsClass) return "'" +SafeString(value.ToString())+ "'";
            return value.ToString();
            
        }
    }
}
