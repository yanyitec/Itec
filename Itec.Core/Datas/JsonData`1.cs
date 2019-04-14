using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public class JsonData<T> :JsonData, IData<T>
        where T:class
    {
        public JsonData(JObject data = null) : base(data)
        {
            
        }

        public JsonData(string json):base(json)
        {
            
        }

        public T ToObject()
        {
            return base.Internals.ToObject<T>();
        }
    }
}
