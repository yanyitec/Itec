using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Itec.ORMs
{
    public class DbClass<T> : MetaClass<T>, IDbClass, IDbClass<T>
    {
        public DbClass(Func<JObject> configGetter) : base(configGetter) {
            
        }

        protected override IMetaProperty CreateProperty(MemberInfo memberInfo)
        {
            return new DbProperty<T>(memberInfo, this);
        }

        IDbProperty _IdField;
        public IDbProperty PrimaryProperty{
            get{
                if(_IdField==null){
                    lock(this){
                        if(_IdField==null) _IdField = GetPrimaryProperty();
                    }
                }
                return _IdField;
            }
        }
        public IDbProperty GetPrimaryProperty(){
             var clsName = this.Name;
            if(clsName.EndsWith("Entity")) clsName = clsName.Substring(0,clsName.Length-"Entity".Length);
            else if(clsName.EndsWith("Class")) clsName = clsName.Substring(0,clsName.Length-"Class".Length);
            var tbName = this.GetTablename();
            var idName1 = clsName.ToLower() + "id";
            var idName2 = clsName.ToLower() + "id";
                            
            IDbProperty idprop = null;
            foreach(var prop in this.FieldedProps){
                var n = prop.Key.ToLower();
                if(n=="id") {idprop = prop.Value;break;}
                if(n==idName1 || n== idName2 && idprop==null){
                    idprop = prop.Value;
                }
            }
            return idprop;
        }

        string _tbName;
        public string Tablename{
            get{
                if(_tbName==null){
                    lock(this){
                        if(_tbName==null) _tbName = GetTablename();
                    }
                }
                return _tbName;
            }
        }

        public string GetTablename()
        {

            return this.GetAttribute<DbTableAttribute>()?.Name ?? this.Name;
            
        }


        Dictionary<string, IDbProperty> _FieldedProps;
        public IReadOnlyDictionary<string, IDbProperty> FieldedProps
        {
            get
            {
                if (_FieldedProps == null)
                {
                    lock (this)
                    {
                        if (_FieldedProps == null)
                        {
                            _FieldedProps = new Dictionary<string, IDbProperty>();
                            foreach (var pair in this.Props)
                            {
                                var prop = pair.Value as IDbProperty;
                                if (prop.Field != null)
                                {
                                    _FieldedProps.Add(prop.Field.Name, prop);
                                }

                            }

                        }
                    }
                }
                return _FieldedProps;
            }
        }

    }
}
