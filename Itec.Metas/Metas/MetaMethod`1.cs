using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class MetaMethod<T>:MetaMethod
    {
        public MetaMethod(MethodInfo method, MetaClass<T> cls) : base(method, cls) {

        }

        //Func<T,Models.IReadonlyModel, object> _DoCall;

        //public object Call(T instance,Models.IReadonlyModel valueProvider)
        //{
        //    if (_DoCall == null)
        //    {
        //        lock (this)
        //        {
        //            if (_DoCall == null) _DoCall = MakeCall<T>(this);
        //        }

        //    }
        //    return _DoCall(instance,valueProvider);
        //}

        
    }
}
