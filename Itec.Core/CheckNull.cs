using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec
{
    public static class CheckNull
    {
        public static bool IsNull(this object self) {
            if (self == null) return true;
            var type = self.GetType();
            var t = type.FullName;
            if (t.StartsWith("System.Nullable`1") || t.StartsWith("Itec.Noneable`1"))
            {
                var innerChecker = Checkers.GetOrAdd(t, (tn) => MakeNullableChecker(type));
                return innerChecker(self);
            }
            return false;
        }

        public static bool IsNullOrEmpty(this object self) {
            var isNull = IsNull(self);
            if (isNull) return true;
            if (self is string) {
                return string.IsNullOrWhiteSpace(self.ToString());
            }
            var en = self as System.Collections.IEnumerable;
            if (en != null)
            {
                foreach (var item in en)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static ConcurrentDictionary<string, Func<object, bool>> Checkers = new ConcurrentDictionary<string, Func<object, bool>>() {
            //{ typeof(byte?).ToString(),new Func<object, bool>((value)=>((byte?)value).HasValue) }
            //,{ typeof(short?).ToString(),new Func<object, bool>((value)=>((short?)value).HasValue) }
            //,{ typeof(ushort?).ToString(),new Func<object, bool>((value)=>((ushort?)value).HasValue) }
            //,{ typeof(char?).ToString(),new Func<object, bool>((value)=>((char?)value).HasValue) }
            //,{ typeof(bool?).ToString(),new Func<object, bool>((value)=>((bool?)value).HasValue) }
            //,{ typeof(int?).ToString(),new Func<object, bool>((value)=>((int?)value).HasValue) }
            //,{ typeof(uint?).ToString(),new Func<object, bool>((value)=>((uint?)value).HasValue) }
            //,{ typeof(float?).ToString(),new Func<object, bool>((value)=>((float?)value).HasValue) }
            //,{ typeof(double?).ToString(),new Func<object, bool>((value)=>((double?)value).HasValue) }
            //,{ typeof(long?).ToString(),new Func<object, bool>((value)=>((long?)value).HasValue) }
            //,{ typeof(ulong?).ToString(),new Func<object, bool>((value)=>((ulong?)value).HasValue) }
            //,{ typeof(decimal?).ToString(),new Func<object, bool>((value)=>((decimal?)value).HasValue) }
            //,{ typeof(DateTime?).ToString(),new Func<object, bool>((value)=>((DateTime?)value).HasValue) }
            //,{ typeof(Guid?).ToString(),new Func<object, bool>((value)=>((Guid?)value).HasValue) }
        };
        
        static Func<object, bool> MakeNullableChecker(Type type) {
            var valueExpr = Expression.Parameter(typeof(object),"value");
            var convertExpr = Expression.Convert(valueExpr, type);
            var hasValueExpr = Expression.PropertyOrField(convertExpr,"HasValue");
            var lamda = Expression.Lambda<Func<object, bool>>(hasValueExpr,valueExpr);
            return lamda.Compile();
        }

        public static Func<object, bool> GetChecker(Type type) {
            var t = type.FullName;
            if (t.StartsWith("System.Nullable`1") || t.StartsWith("Itec.Noneable`1")) {
                var innerChecker = Checkers.GetOrAdd(t,(tn)=> MakeNullableChecker(type));
                return new Func<object, bool>((value)=> value==null?true:innerChecker(value));
            }
            return new Func<object, bool>((value)=>value==null);
        }
    }
}
