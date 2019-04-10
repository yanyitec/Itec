using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class MetaMethods:IEnumerable<MetaMethod>
    {
        public List<MetaMethod> _List;
        public MetaMethods(MetaClass cls,MethodInfo info=null) {
            this.Class = cls;
            this._List = new List<MetaMethod>();
            if (info != null) this._List.Add(new MetaMethod(info,cls));
        }
        public MetaClass Class { get; private set; }

        internal void Add(MetaMethod m) {
            this._List.Add(m);
        }

        //public object Call(object instance, Models.IReadonlyModel provider)
        //{
        //    var method = this._List.First();
        //    return method.Call(instance,provider);
        //}

        public MetaMethod FindMethod(IEnumerable<Type> argTypes) {
            var argList = argTypes.ToList();
            var resultScore = 0;
            MetaMethod result = null;
            foreach (var method in _List) {
                var score = 0;
                if (method.Parameters.Count < argList.Count) continue;
                bool isMatch = true;
                for (var i = 0; i < argList.Count; i++) {
                    var methodParamType = method.Parameters[i].ParameterType;
                    var argType = argList[i];
                    if (methodParamType.IsAssignableFrom(argType))
                    {
                        score++;
                        if (methodParamType == argType) score++;
                    }
                    else {
                        var methodParamActualType = methodParamType.FullName.StartsWith("System.Nullable`1") ? methodParamType.GetGenericArguments()[0] : methodParamType;
                        var argActualType = argType.FullName.StartsWith("System.Nullable`1")? argType.GetGenericArguments()[0] : argType;
                        if (argActualType != methodParamType)
                        {
                            isMatch = false;
                            break;
                        }
                        else {
                            score++;
                        }
                    }
                }
                if (!isMatch) continue;
                if (method.Parameters.Count > argList.Count) {
                    
                    for (var i = argList.Count; i < method.Parameters.Count; i++) {
                        if (!method.Parameters[i].HasDefaultValue) {
                            isMatch = false;
                            break;
                        }
                    }
                    if (!isMatch) continue;
                }
                if (resultScore < score)
                {
                    result = method;
                    resultScore = score;
                }
                if (resultScore == score)
                {
                    if (method.Parameters.Count == argList.Count) {
                        result = method;
                    }
                }
            }
            return result;
            
        }

        public IEnumerator<MetaMethod> GetEnumerator()
        {
            return this._List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._List.GetEnumerator();
        }
    }
}
