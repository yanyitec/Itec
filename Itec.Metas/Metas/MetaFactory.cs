using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Itec.Metas
{
    public class MetaFactory
    {
        public static MetaFactory Default = new MetaFactory();
        ConcurrentDictionary<Guid, MetaClass> _Classes;
        public MetaFactory() {
            this._Classes = new ConcurrentDictionary<Guid, MetaClass>();
        }
        public MetaClass GetClass(Type type) {
            return _Classes.GetOrAdd(type.GUID,(k)=> {
                var t = typeof(MetaClass<>).MakeGenericType(type);
                return Activator.CreateInstance(t, type) as MetaClass;
            });
        }
    }
}
