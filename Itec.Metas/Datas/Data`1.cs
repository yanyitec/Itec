using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Itec.Datas
{
    public class Data<T>:DynamicObject,IData<T>
        where T : class
    {
        protected Func<T> GetObject { get; set; }
        public MetaClass<T> MetaClass { get; private set; }
        protected Data(IMetaFactory metaFactory = null)
        {
            
            var factory = metaFactory ?? MetaFactory.Default;
            this.MetaClass = factory.GetClass<T>();
        }

        protected Data(MetaClass<T> cls) {
            this.MetaClass = cls;
        }

        public Data(Func<T> getObject, IMetaFactory metaFactory = null):this(metaFactory)
        {
            GetObject = getObject;
            
        }

        public Data(T data,IMetaFactory metaFactory = null)
            :this(() => data,metaFactory) {
            
        }

        

        public string this[string key] {
            get {
                return this.GetString(key);
            }
        }

        public bool Equals(IData other)
        {
            throw new NotImplementedException();
        }

        public T1 Get<T1>(string key = null)
        {
            if (key == null) {
                if (typeof(T1).IsAssignableFrom(typeof(T))) {
                    return this.MetaClass.ConvertTo<T1>(this.GetObject());
                } else return default(T1);
            }
            var ins = this.GetObject();
            var prop = this.MetaClass[key];
            if (prop == null) return default(T1);

            return (T1)prop.GetValue(ins);
        }

        public object Get(Type objectType, string key = null)
        {
            if (key == null)
            {
                if (objectType.IsAssignableFrom(typeof(T)))
                {
                    return this.GetObject();
                }
                else return null;
            }
            var ins = this.GetObject();
            var prop = this.MetaClass[key];
            if (prop == null) return null;

            var result = prop.GetValue(ins);
            if (result == null) return null;
            if (objectType.IsAssignableFrom(result.GetType())) return result;
            return null;
        }

        public IReadOnlyList<string> GetMemberNames()
        {
            return this.MetaClass.PropNames.ToList();
        }

        

        public object GetRaw(string key = null)
        {
            if (key == null)
            {
                return this.GetObject();
            }
            var ins = this.GetObject();
            var prop = this.MetaClass[key];
            if (prop == null) return null;

            return prop.GetValue(ins);
        }

        public string GetString(string key = null)
        {
            var obj = this.GetRaw();
            return obj?.ToString();
        }

        public string ToJSON()
        {
            var obj = this.GetRaw();
            if (obj == null) return null;
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public T ToObject()
        {
            return GetObject == null ? default(T) : GetObject();
        }

        #region dynamic
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.GetMemberNames();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Get(binder.ReturnType, binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var key = indexes[0];

            result = this.Get(binder.ReturnType, key == null ? null : key.ToString());
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var obj = this.GetRaw();
            if (binder.ReturnType.IsByRef) {
                if (obj == null) {
                    result = null;
                    return true;
                }
                if (binder.ReturnType.IsAssignableFrom(obj.GetType())) {
                    result = obj;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            throw new InvalidOperationException("不能当作函数使用");
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            throw new InvalidOperationException("成员不能当作函数使用，成员不能被调用");
        }



        #endregion
    }
}
