using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itec.ORMs.SQLs
{
    public class SQL<T>
    {
        public SQL(string membersString, Database db, IDbClass dbClass)
        {
            this.Database = db;
            this.DbSettings = db.Settings;
            this.DbTrait = db.Trait;
            this.DbClass = dbClass;
            this.FieldedProps = this.DbClass.FieldedProps;

            this.MembersString = membersString??string.Empty;
            if (string.IsNullOrEmpty(this.MembersString) || this.MembersString == "*")
            {
                this._AllowedProps = this.DbClass.FieldedProps;
                this.MembersString = "*";
            }
            else {
                var memberNames = MembersString.Split(',');
                var props = new Dictionary<string, IDbProperty>();
                _AllowedProps = props;
                foreach (var mName in memberNames)
                {
                    var memName = mName.Trim();
                    IDbProperty prop = null;
                    if (this.DbClass.FieldedProps.TryGetValue(memName, out prop))
                    {
                        props.Add(prop.Name, prop);
                    }

                }
            }
            

            

            this.Create = new Create<T>(this);
            this.Insert = new Insert<T>(this);
            this.Select = new Select<T>(this);
            this.Count = new Count<T>(this);
            this.GetById = new GetById<T>(this);
            //this.Update = new Update(model, membersString);
            //this.Select = new Select(model, membersString);
        }
        public Database Database { get; private set; }

        public IDbClass DbClass { get; private set; }


        public DbSettings DbSettings { get; private set; }

        public DbTrait DbTrait { get; private set; }

        public IReadOnlyDictionary<string, IDbProperty> FieldedProps { get; private set; }

        public string MembersString { get; private set; }
        IReadOnlyDictionary<string, IDbProperty> _AllowedProps;
        public IReadOnlyDictionary<string, IDbProperty> AllowedProps
        {
            get
            {
                
                return _AllowedProps;
            }
        }

        public string Tablename( bool withSqlChar = false)
        {

            string tbName = this.DbClass.GetAttribute<DbTableAttribute>()?.Name ?? this.DbClass.Name;
            var prefix = this.Database.Settings.TablePrefix;
            if (!string.IsNullOrWhiteSpace(prefix)) tbName = prefix + tbName;
            if (!withSqlChar) return tbName;
            return this.DbTrait.SqlTablename(tbName);
        }

        public Create<T> Create { get; private set; }
        public Insert<T> Insert { get; private set; }
        public Select<T> Select { get; private set; }

        public Count<T> Count { get; private set; }

        public GetById<T> GetById { get; private set; }

        //public Update Update { get; private set; }



        //public Where Where { get; private set; }

        

        public static string SafeString(string str)
        {
            return str.Replace("'", "''");//.Replace("\n","\\n").Replace("\r", "\\r");
        }

        public static string SqlValue(object value, bool nullable = false, object defaultValue = null)
        {
            if (value == null)
            {
                if (nullable) return "NULL";
                else value = defaultValue ?? "";
            }
            var t = value.GetType();
            if (t == typeof(DateTime)) return "'1790-1-1'";
            if (t.IsClass) return "'" + SafeString(value.ToString()) + "'";
            return value.ToString();

        }
    }
}
