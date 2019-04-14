using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Itec.Entities
{
    public class EntityProperty:MetaProperty
    {
        public EntityProperty(MemberInfo memberInfo, EntityClass cls = null) : base(memberInfo, cls)
        {

        }
        FilterTypes? _FilterType;
        public FilterTypes FilterType {
            get {
                if (_FilterType == null) {
                    lock (this) {
                        if (_FilterType == null) {
                            var attr = this.GetAttribute<FilterAttribute>();
                            if (attr != null) _FilterType = attr.FilterType;
                            else _FilterType = FilterTypes.Equal;
                        }
                    }
                }
                return _FilterType.Value;
            }
        }
    }
}
