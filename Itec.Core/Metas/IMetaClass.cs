using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Itec.Metas
{
    public interface IMetaClass
    {
        IMetaProperty this[string name] { get; }

        IReadOnlyList<Attribute> Attributes { get; }
        Func<JObject> GetConfig { get; }
        string Name { get; }
        IReadOnlyList<string> PropNames { get; }
        Type Type { get; }

        IEnumerable<MetaMethods> AsMethodsEnumerable();
        object Clone(object src);
        T ConvertTo<T>(object instance = null);
        object CopyTo(object src, object dest, string fieldnames = null);
        object CopyTo(Type targetType, object src, object dest = null, string fieldnames = null);
        object CreateInstance();
        T GetAttribute<T>() where T : Attribute;
        IEnumerator<IMetaProperty> GetEnumerator();
        MetaMethods GetMethods(string name);
        object GetValue(object obj, string name);
        IMetaClass SetValue(object obj, string name, object value);
    }
}