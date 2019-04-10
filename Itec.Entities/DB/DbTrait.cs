
using Itec.Metas;
using Itec.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Entities.DB
{
    /// <summary>                                                                
    /// 数据库特性
    /// mysql/sqlserver等一个数据库一种特性
    /// </summary>
    public class DbTrait
    {
        public IDictionary<Guid, DbType> DataTypeMaps { get;protected set; }

        public SqlParametricKinds ParametricKind { get;protected set; }

        public virtual char NameStart { get; protected set; }
        public virtual char NameEnd { get; protected set; }

        public virtual DbConnection CreateConnection(string connString) {
            return new System.Data.SQLite.SQLiteConnection(connString);
        }

        public virtual DbCommand CreateDbCommand(DbConnection conn, string cmdText) {
            return new System.Data.SQLite.SQLiteCommand(cmdText, conn as System.Data.SQLite.SQLiteConnection);
        }

        public string PrimaryKeyword { get; set; }

        public virtual string CheckTableExistsSql(EntityModel model) {
            var tbName = this.MakeSqlTablename(model);
            return $"SELECT COUNT(*) FROM sqlite_master where type='table' and name='${tbName}'";
        }

        

        public string MakeSqlFieldname(string fname) {
            if (string.IsNullOrEmpty(fname)) return fname;
            var rs = string.Empty;
            if (NameStart != '\0') rs += NameStart;
            rs += fname;
            if (NameEnd != '\0') rs += NameEnd;
            return rs;
        }

        public string MakeSqlFieldname(MetaProperty prop) {
            var fieldname = GetFieldname(prop);
            return MakeSqlFieldname(fieldname);
        }

        public virtual DbType GetDbType(MetaProperty prop) {
            DbType dbType = DbType.String;
            if (this.DataTypeMaps != null && this.DataTypeMaps.Count > 0) {
                this.DataTypeMaps.TryGetValue(prop.NonullableType.GUID,out dbType);
            }
            return dbType;
        }

        public virtual string GetSqlFieldType(Type type) {
            var hash = type.GetHashCode();
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
            if (typeof(Guid).GetHashCode() == hash) return "CHAR(64)";
            if (typeof(DateTime).GetHashCode() == hash) return "DATETIME";
            return "TEXT";
        }

        public string GetSqlFieldTypeAndPrecision(EntityProperty prop) {
            var type = prop.NonullableType;
            var dbType = this.GetSqlFieldType(type);
            if (dbType == "VARCHAR") return dbType + "(50)";
            return dbType;
        }

        public string MakeSqlTablename(MetaClass cls,string tbPrefix=null)
        {
            var tbAttr = cls.GetAttribute<TableAttribute>();
            var tbName = tbAttr?.Name ?? cls.Name;
            if (!string.IsNullOrWhiteSpace(tbPrefix)) tbName = tbPrefix + tbName;
            var rs = string.Empty;
            if (NameStart != '\0') rs += NameStart;
            rs += tbName;
            if (NameEnd != '\0') rs += NameEnd;
            return rs;
        }

        
      

        
        public static string GetFieldname(MetaProperty prop) {
            if (prop.GetAttribute<NotFieldAttribute>() != null) return null;
            var fieldAttr = prop.GetAttribute<FieldAttribute>();
            var fname = fieldAttr?.Name??prop.Name;

            
            return fname;
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
