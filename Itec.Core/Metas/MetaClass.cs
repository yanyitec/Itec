using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Itec.Metas
{
    public class MetaClass : IEnumerable<IMetaProperty>, IMetaClass
    {
        public MetaClass(Type type,Func<JObject> configGetter = null) {
            this.Type = type;
            this.GetConfig = configGetter;
        }
        public Func<JObject> GetConfig { get; private set; }
        void Init() {
            this._Props = new Dictionary<string, IMetaProperty>();
            this._Methods = new Dictionary<string, MetaMethods>();
            this._PropNames = new List<string>();
            var members = this.Type.GetMembers();
            
            foreach (var member in members) {
                
                if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                {
                    var prop = this.CreateProperty(member);
                    if (prop != null)
                    {
                        this._Props.Add(prop.Name, prop);
                        this._PropNames.Add(prop.Name);
                    }
                }
                else if (member.MemberType == MemberTypes.Method) {
                    
                    var method = this.CreateMethod(member as MethodInfo);
                    if (method != null) {
                        MetaMethods ms = null;
                        if (!this._Methods.TryGetValue(member.Name, out ms)) {
                            ms = new MetaMethods(this);
                            _Methods.Add(member.Name,ms);
                        }
                        ms.Add(method);
                    }

                }
            }
        }

        public object CreateInstance() {
            return Activator.CreateInstance(this.Type);
        }

        

        public Type Type { get; private set; }

        public string Name {
            get { return this.Type.Name; }
        }

        Dictionary<string, IMetaProperty> _Props;

        List<string> _PropNames;
        public IReadOnlyList<string> PropNames {
            get {
                if (_PropNames == null)
                {
                    lock (this)
                    {
                        if (_PropNames == null)
                        {
                            this.Init();
                        }
                    }
                }
                return _PropNames;
            }
        }

        protected IReadOnlyDictionary<string, IMetaProperty> Props {
            get {
                if (_Props == null)
                {
                    lock (this)
                    {
                        if (_Props == null)
                        {
                            this.Init();
                        }
                    }
                }
                return _Props;
            }
        }

        public object GetValue(object obj, string name) {
            return this[name].GetValue(obj);
        }

        public IMetaClass SetValue(object obj, string name, object value) {
            this[name].SetValue(obj,value);
            return this;
        }


        public IMetaProperty this[string name] {
            get {
                if (_Props == null) {
                    lock (this) {
                        if (_Props == null) {
                            this.Init();
                        }
                    }
                }
                IMetaProperty prop = null;
                this._Props.TryGetValue(name,out prop);
                return prop;
            }
        }

        Dictionary<string, MetaMethods> _Methods;

        public MetaMethods GetMethods(string name) {
            if (_Methods == null) {
                lock (this) {
                    if (_Methods == null) {
                        this.Init();
                    }
                }
            }
            MetaMethods result = null;
            _Methods.TryGetValue(name,out result);
            return result;
        }

        public IEnumerable<MetaMethods> AsMethodsEnumerable() {
            return this._Methods.Values;
        }

        List<Attribute> _Attributes;
        public IReadOnlyList<Attribute> Attributes
        {
            get
            {
                if (_Attributes == null)
                {
                    lock (this)
                    {
                        if (_Attributes == null)
                        {
                            _Attributes = new List<Attribute>(this.Type.GetCustomAttributes());
                        }
                    }
                }
                return _Attributes;
            }
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return this.Attributes.FirstOrDefault(p => p.GetType() == typeof(T)) as T;
        }

        public T ConvertTo<T>(object instance = null) {
            if (instance == null) instance = this.CreateInstance();
            return (T)instance;
        }

        

        

        

        public object CopyTo(Type targetType, object src, object dest=null, string fieldnames = null) {
            var copier = this.GetCopier(targetType,fieldnames);
            return copier.Copy(src, dest);
        }

        public object CopyTo( object src, object dest, string fieldnames = null)
        {
            return CopyTo(dest.GetType(), src, dest, fieldnames);
        }

        public object Clone(object src) {
            if (src == null) return null;
            if (src.GetType() != this.Type) throw new InvalidProgramException("克隆对象的类型不正确");
            return CopyTo(this.Type,src, null, null);
        }



        ConcurrentDictionary<string, ConcurrentDictionary<string, Copier>> _Copiers;

        protected Copier GetCopier(Type targetType, string fieldnames) {
            if (_Copiers == null) {
                lock (this) {
                    if (_Copiers == null) _Copiers = new ConcurrentDictionary<string, ConcurrentDictionary<string, Copier>>();
                }
            }
            ConcurrentDictionary<string, Copier> destCopiers = _Copiers.GetOrAdd(targetType.Name, (typeFullname) =>new ConcurrentDictionary<string, Copier>());
            return destCopiers.GetOrAdd(fieldnames == null ? string.Empty : fieldnames, (fldnms) =>
            {
                var type = typeof(Copier<,>);
                var ctype = type.MakeGenericType(this.Type, targetType);
                return Activator.CreateInstance(ctype, fldnms) as Copier;
            });
        }




        protected virtual IMetaProperty CreateProperty(MemberInfo memberInfo)
        {
            var t = typeof(MetaProperty<>).MakeGenericType(this.Type);
            return Activator.CreateInstance(t, memberInfo, this) as IMetaProperty;
        }

        protected virtual MetaMethod CreateMethod(MethodInfo methodInfo)
        {
            if (methodInfo.IsPrivate) return null;
            //return new MetaMethod(methodInfo, this);
            var t = typeof(MetaMethod<>).MakeGenericType(this.Type);
            return Activator.CreateInstance(t, methodInfo, this) as MetaMethod;
        }

        
        public IEnumerator<IMetaProperty> GetEnumerator()
        {
            return this.Props.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Props.Values.GetEnumerator();
        }
    }
}
