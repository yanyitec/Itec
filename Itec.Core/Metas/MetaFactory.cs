using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Itec.Metas
{
    public class MetaFactory : IMetaFactory
    {
        public static MetaFactory Default = new MetaFactory();
        ConcurrentDictionary<Guid, MetaClass> _Classes;
        public MetaFactory() {
            this._Classes = new ConcurrentDictionary<Guid, MetaClass>();
        }
        protected virtual Type MetaClassType{ get { return typeof(MetaClass<>); } }
        public MetaClass GetClass(Type type) {
            return _Classes.GetOrAdd(type.GUID,(k)=> {
                var t = MetaClassType.MakeGenericType(type);
                Func<JObject> configGetter = () => this.LoadConfig(type);
                return Activator.CreateInstance(t, configGetter) as MetaClass;
            });
        }

        public MetaClass<T> GetClass<T>() {
            return _Classes.GetOrAdd(typeof(T).GUID, (k) => {
                var t = MetaClassType.MakeGenericType(typeof(T));
                Func<JObject> configGetter = () => this.LoadConfig(typeof(T));
                return Activator.CreateInstance(t,  configGetter) as MetaClass;
            }) as MetaClass<T>;
        }

        string _ConfigPath;
        public string ConfigPath
        {
            get
            {
                if (_ConfigPath == null)
                {
                    lock (this)
                    {
                        if (_ConfigPath == null)
                        {
                            _ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/Configs");
                        }
                    }
                }
                return _ConfigPath;
            }
        }

        public JObject LoadConfig(Type clsType)
        {
            var basePath = this.ConfigPath;
            var assName = clsType.Assembly.FullName;
            var moduleName = Path.Combine(basePath, assName);
            var clsName = Path.Combine(moduleName, clsType.FullName) + ".json";
            var sb = new StringBuilder();
            if (File.Exists(clsName))
            {
                var lines = File.ReadLines(clsName);
                foreach (var line in lines)
                {
                    int at = line.IndexOf("//");
                    if (at >= 0) sb.Append(line.Substring(0, at));
                    else sb.Append(line);

                }
                var jsonText = sb.ToString();
                return JObject.Parse(jsonText);
            }
            else return null;

        }
    }
}
