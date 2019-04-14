
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Itec.Metas;
using Newtonsoft.Json.Linq;

namespace Itec.ORMs
{
    /// <summary>
    /// 实体集
    /// </summary>
    public class DbSet<T> : Datas.Dataset<T>, IDbSet where T:class
    {
        public Database Database { get; private set; }

        public IDbClass DbClass { get; private set; }

        ConcurrentDictionary<string, SQLs.SQL<T>> FieldedSqls { get; set; }

        public DbSet(Database db,IMetaFactory classFactory = null):base(classFactory?? DbClassFactory.Default) {
            this.Database = db;
            
            this.DbClass = this.MetaClass as IDbClass;
            FieldedSqls = new ConcurrentDictionary<string, SQLs.SQL<T>>();
            this.Sql = new SQLs.SQL<T>(string.Empty,db,this.DbClass);
            //this.FieldedSqls.TryAdd(null,this.Sql);
            this.FieldedSqls.TryAdd("", this.Sql);
            this.FieldedSqls.TryAdd("*", this.Sql);
        }

        

        DbSet(DbSet<T> other) : base(other.MetaClass) {
            this.Database = other.Database;
            this.DbClass = other.DbClass;
            this.FieldedSqls = other.FieldedSqls;
            this.Sql = other.Sql;
        }

        DbSet(DbSet<T> other,string membersString) : this(other)
        {
            this.Sql = this.FieldedSqls.GetOrAdd(membersString??string.Empty, (m) => new SQLs.SQL<T>(m,this.Database,this.DbClass));
        }

        public DbSet<T> MembersString(string membersString) {
            if (membersString == this.Sql.MembersString) return this;
            return new DbSet<T>(this, membersString);
        }



        public SQLs.SQL<T> Sql { get; private set; }



        public bool Insert(T data, IDbTransaction trans=null) {
            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return this.Sql.Insert.Execute(data, conn, dbTran);
            }
            else {
                using (var conn = this.Database.CreateConnection()) {
                    conn.Open();
                    return this.Sql.Insert.Execute(data, conn, null);
                }
            }
            
        }
        


    }
}
