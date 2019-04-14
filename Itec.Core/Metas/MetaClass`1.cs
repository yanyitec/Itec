using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class MetaClass<T> : MetaClass, IEnumerable<IMetaProperty<T>>, IMetaClass<T>
    //where T : class
    {
        public MetaClass(Func<JObject> configGetter=null) : base(typeof(T), configGetter) {
            
        }
        
        public new IMetaProperty<T> this[string name] {
            get {
                return base[name] as IMetaProperty<T>;
            }
        }

        public TDest CopyTo<TDest>( T src, TDest dest = default(TDest), string fieldnames = null)
        {
            var copier = this.GetCopier(typeof(TDest), fieldnames) as Copier<T,TDest>;
            return copier.Copy(src, dest);
        }

        

        T Clone(T src)
        {
            return this.CopyTo<T>(src);
        }

        

        protected override IMetaProperty CreateProperty(MemberInfo memberInfo)
        {
            return new MetaProperty<T>(memberInfo,this);
        }
        protected override MetaMethod CreateMethod(MethodInfo methodInfo)
        {
            return new MetaMethod<T>(methodInfo, this);
        }
        IEnumerator<IMetaProperty<T>> IEnumerable<IMetaProperty<T>>.GetEnumerator()
        {
            return new Itec.ConvertEnumerator<IMetaProperty<T>,IMetaProperty>(this.Props.Values.GetEnumerator(),(src)=>(MetaProperty<T>)src);
        }
    }
}
