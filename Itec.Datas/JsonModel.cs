using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Itec.Models
{
    public class JsonModel:ReadonlyJsonModel,IModel
    {
        public JsonModel(JObject data = null) : base(data)
        {
        }

        public JsonModel(string json) : base(json)
        {
        }



        public IModel Remove(string key)
        {
            this.Internals.Remove(key);
            this.HasChanges = true;
            return this;
        }

        public new string this[string key]
        {
            get { return GetString(key); }
            set { SetString(key, value); }
        }



        IModel SetString(string key, string value)
        {
            Internals[key] = value;
            this.HasChanges = true;
            return this;
        }

        


        public IModel Set<T>( T value,string key=null)
        {
            if (key == null)
            {
                this.Internals = JObject.FromObject(value);
                this.HasChanges = true;
                return this;
            }
            var token = JToken.FromObject(value);
            this.Internals[key] = token;
            this.HasChanges = true;
            return this;
        }



        public IModel Set(object value, string key=null)
        {
            if (key == null)
            {
                this.Internals = JObject.FromObject(value);
                this.HasChanges = true;
                return this;
            }
            var token = JToken.FromObject(value);
            this.Internals[key] = token;
            this.HasChanges = true;
            return this;
        }

        public IModel Set(Type objectType, object value, string key = null) {
            if (value != null && !objectType.IsAssignableFrom(value.GetType())) throw new ArgumentException("value与指定的类型不匹配");

            if (key == null)
            {
                this.Internals = JObject.FromObject(value);
                this.HasChanges = true;
                return this;
            }
            var token = JToken.FromObject(value);
            this.Internals[key] = token;
            this.HasChanges = true;
            return this;
        }

        public IModel SetRaw(object value, string key = null)
        {
            if (value == null)
            {
                value = JRaw.CreateNull();
            }
            else {
                if (!(value is JToken)) throw new ArgumentException("SetRaw必须用JToken作为value参数");
            }
            if (key == null)
            {
                this.Internals = value as JObject??new JObject(); 
                this.HasChanges = true;
                return this;
            }
            var token = value as JToken;
            this.Internals[key] = token;
            this.HasChanges = true;
            return this;
        }



        #region dynamic




        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.Set(value, binder.Name);
            return true;
        }



        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var key = indexes[0];

            this.Set(binder.ReturnType,value,key == null ? null : key.ToString());
            return true;
        }

        



        #endregion
    }
}
