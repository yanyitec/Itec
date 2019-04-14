using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Itec.ORMs
{
    public class DbProperty<T> : Metas.MetaProperty<T>, IDbProperty
    {
        public DbProperty(MemberInfo memberInfo,DbClass<T> cls) : base(memberInfo, cls)
        {

        }
        DbField _Field;
        public DbField Field {
            get {
                if (_Field == null) {
                    lock (this) {
                        if (_Field == null) {
                            var nonField = this.GetAttribute<NotDbFieldAttribute>();
                            if (nonField != null)
                            {
                                _Field = DbField.NotDbField;
                            }
                            else {
                                if (this.PropertyType.IsClass && this.PropertyType != typeof(string))
                                {
                                    _Field = DbField.NotDbField;

                                }
                                else {
                                    var cfg = this.Class.GetConfig();
                                    if (cfg != null)
                                    {
                                        var propCfg = cfg[this.Name] as JObject;
                                        _Field = new DbField(this, propCfg);
                                    }
                                    else
                                    {
                                        _Field = new DbField(this, null);
                                    }
                                }
                                
                                
                            }
                        }
                    }
                }
                return _Field == DbField.NotDbField?null:_Field;
            }
        }
    }
}
