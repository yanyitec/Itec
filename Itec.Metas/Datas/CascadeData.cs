using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Itec.Datas
{
    public class CascadeData:DynamicObject,IData
    {
        public CascadeData(IData current = null,IData super = null)
        {
            this.Super = super;
            this.Current = current;
        }

        public IData Super { get; private set; }

        public IData Current { get; private set; }

        public string this[string key]
        {
            get { return this.GetString(key); }
        }



        public string GetString(string key = null)
        {
            var value = this.Current.GetString(key);
            if (value == null && this.Super!=null) value = this.Super.GetString(key);
            return value;
        }



        public object GetRaw(string key)
        {
            var value = this.Current.GetRaw(key);
            if (value == null && this.Super != null) value = this.Super.GetRaw(key);
            return value;
        }





        public T Get<T>(string key = null)
        {
            var value = this.Get(typeof(T),key);
            return (T)value;
        }




        public object Get(Type type, string key = null)
        {
            var value = this.Current.Get(type,key);
            if (value== null && this.Super != null) value = this.Super.Get(type,key);
            return value;
        }

        public string ToJSON()
        {
            return this.Current.ToJSON();
        }

        public IReadOnlyList<string> GetMemberNames()
        {
            var nms = this.Current.GetMemberNames();
            
            if (this.Super != null) {
                var names = nms as List<string>;
                if (names == null) {
                    names = new List<string>();
                    foreach (var n in nms) names.Add(n);
                }
                var superNames = this.Super.GetMemberNames();
                foreach (var name in superNames) {
                    if (names.Contains(name)) names.Add(name);
                }
            }
            return nms;
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
            result = this.Get(binder.ReturnType);
            return true;
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

        public bool Equals(IData other)
        {
            if (other == this) return true;
            if (this.Current.Equals(other) ) return true;
            if (this.Super != null || this.Super.Equals(other)) return true;
            return false;
        }
    }
}
