using Itec.Entities.DB;
using Itec.Entities.SQLs;
using Itec.Metas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Entities
{
    public class EntityClass:Metas.MetaClass
    {
        public EntityClass(DB.Database db,Type type) : base(type) {
            this.Database = db;
            FieldedModels = new ConcurrentDictionary<string, EntityClass>();
            this.Sqls = new Sqls(null,this);
            FieldedModels.TryAdd("*",this);
            this.InitMembers();
        }

        protected EntityClass(DB.Database db,Type type,string membersString) : base(type)
        {
            _MembersString = membersString;
            this.Sqls = new Sqls(membersString, this);
            this.InitMembers();
        }

        void InitMembers() {
            if (_MembersString == null)
            {
                var members = new List<EntityProperty>();
                foreach (EntityProperty member in base.Props.Values) {
                    members.Add(member);
                }
                this.Members = members;
            }
            else {
                var fields = this._MembersString.Split(',');
                var members = new List<EntityProperty>();
                foreach (EntityProperty member in base.Props.Values)
                {
                    if(fields.Contains(member.Name))members.Add(member);
                }
                this.Members = members;
            }
        }

        public Database Database { get; protected set; }

        protected override MetaProperty CreateProperty(MemberInfo memberInfo)
        {
            return new EntityProperty(memberInfo, this);
        }

        protected virtual EntityClass Clone(string memberString) {

            return new EntityClass(this.Database,this.Type,memberString) {
                FieldedModels = this.FieldedModels
            };
        }

        public Sqls Sqls {
            get;
            private set;
        }

        

        #region member names
        string _MembersString;
        public string MembersString(){
                return _MembersString;           
        }

        public EntityClass MembersString(string membersString) {
            return FieldedModels.GetOrAdd(membersString??"*",(str)=>this.Clone(membersString));
        }

        public IReadOnlyList<EntityProperty> Members {
            get;private set;
        }

        #endregion

        protected ConcurrentDictionary<string,EntityClass> FieldedModels { get;  set; }

        
        public static EntityClass<T> GetModel<T>(string dbName = null)
            where T:class
        {
            return Database.GetDbSet<T>(dbName);
        }
    }
}
