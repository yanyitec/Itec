using System.Collections.Concurrent;

namespace Itec.ORMs.SQLs
{
    public class SQLBag<T>
    {
        public Database Database { get; private set; }

        public IDbClass<T> DbClass { get; private set; }



        ConcurrentDictionary<string, SQLs.SQL<T>> InternalSqls { get; set; }

        public SQLs.SQL<T> AllFieldsSql { get; protected set; }

        //IDbClass<T> IDbSet<T>.DbClass => this.DbClass as IDbClass<T>;

        public SQLBag(Database db, IDbClass<T> dbCls)
        {
            this.Database = db;

            this.DbClass = dbCls;
            InternalSqls = new ConcurrentDictionary<string, SQLs.SQL<T>>();
            this.AllFieldsSql = new SQLs.SQL<T>(string.Empty,this);
            //this.FieldedSqls.TryAdd(null,this.Sql);
            this.InternalSqls.TryAdd("", this.AllFieldsSql);
            this.InternalSqls.TryAdd("*", this.AllFieldsSql);
            

        }


        public SQLs.SQL<T> GetSQLObject(string membersString)
        {
            return this.InternalSqls.GetOrAdd(membersString, (ms) => new SQLs.SQL<T>(ms, this));
        }

        public string Tablename(bool withSqlChar = false)
        {

            string tbName = this.DbClass.Tablename;
            var prefix = this.Database.Settings.TablePrefix;
            if (!string.IsNullOrWhiteSpace(prefix)) tbName = prefix + tbName;
            if (!withSqlChar) return tbName;
            return this.Database.Trait.SqlTablename(tbName);
        }

        public Create<T> Create { get { return this.AllFieldsSql.Create; } }



    }
}
