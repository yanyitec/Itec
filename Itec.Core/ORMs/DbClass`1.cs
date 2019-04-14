using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Itec.ORMs
{
    public class DbClass<T>:MetaClass<T>,IDbClass
    {
        public DbClass(Func<JObject> configGetter) : base(configGetter) {
            
        }

        protected override MetaProperty CreateProperty(MemberInfo memberInfo)
        {
            return new DbProperty<T>(memberInfo, this);
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
