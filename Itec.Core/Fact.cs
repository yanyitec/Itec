using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Itec
{
    public static class Fact
    {
        public class FactException :Exception{
            public FactException(string message) : base(message) {
            }
        }

        public static bool IsSilence { get; set; }

        //public static Exception LastException { get; set; }

        public static void Test(Type type,IEnumerable<string> includes=null,IEnumerable<string> excludes=null) {
            Console.WriteLine("@CLASS<" + type.FullName + ">:");
            var instance = Activator.CreateInstance(type);
            var methods = type.GetMethods();
            var pars = new object[] { };
            foreach (var method in methods) {
                if (method.GetCustomAttribute<FactAttribute>() == null) continue;
                if (!CheckName(method.Name, includes, excludes)) continue;
                if (method.GetParameters().Length != 0) throw new InvalidProgramException("FactAttribute只能标记无参数函数");
                Console.WriteLine("@METHOD[" + method.Name + "]:");
                if (IsSilence)
                {
                    try
                    {
                        Invoke(method,instance, pars);
                    }
                    catch (Exception ex)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Failed.");
                        Console.WriteLine(ex.Message);
                        Console.ForegroundColor = color;
                    }
                }
                else {
                    Invoke(method,instance, pars);
                }

                var color1 = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success.");
                
                Console.ForegroundColor = color1;
            }
            Console.WriteLine("@END_CLASS<" + type.FullName + ">");
        }

        public static void Test(Assembly assembly, IEnumerable<string> includes = null, IEnumerable<string> excludes = null)
        {
            
            Console.Write("@ASSEMBLY<" + assembly.FullName + ">");
            
            var types = assembly.GetExportedTypes();
            var pars = new object[] { };
            foreach (var type in types)
            {
                if (type.GetCustomAttribute<FactAttribute>() == null) continue;
                if (!CheckName(type.FullName, includes, excludes)) continue;
                Test(type,includes,excludes);
            }
            Console.WriteLine("@END_ASSEMBLY<" + assembly.FullName + ">");
        }

        public static void Test(IEnumerable<string> includes = null, IEnumerable<string> excludes = null) {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms) {
                if (asm.FullName.StartsWith("System.")) continue;
                if (!CheckName(asm.FullName, includes, excludes)) continue;
                Test(asm,includes,excludes);
            }
        }

        static void Invoke(MethodInfo method, object instance,object[] pars=null) {
            var returnType = method.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType)) {
                
                var task = method.Invoke(instance, pars ?? new object[] { }) as Task;
                if (task.Status == TaskStatus.Faulted)
                {
                    if (task.Exception != null) throw task.Exception;
                    return;
                }
                if (task.Exception != null) return;
                if (task.IsCanceled) return;
                if (!task.IsCompleted) task.Wait();
                
                //ret.GetAwaiter();
                //ret.RunSynchronously();
            }
            else method.Invoke(instance,pars?? new object[] { });
        }

        static bool CheckName(string name, IEnumerable<string> includes = null, IEnumerable<string> excludes = null) {
            if (excludes != null)
            {
                foreach (var exc in excludes)
                {
                    if (name.Contains(exc))
                    {
                        return false;
                    }
                }
                
            }
            if (includes != null)
            {
                bool isIncludes = false;
                foreach (var exc in includes)
                {
                    if (name.Contains(exc))
                    {
                        isIncludes = true;
                        break;
                    }
                }
                if (!isIncludes) return false;
            }
            return true;
        }

        




        public static void Equal<T>(T expected, T actual, string message = null) where T:class{
            if (expected ==actual) {
                return;
            }
            if (expected != null && expected.Equals(actual)) return;
            message = message ?? $"期望两个值相等.期望:{expected},实际:{actual}.";
            throw new FactException(message);
        }

        

        public static void NotEqual<T>(T expected, T actual, string message = null) where T : class
        {
            if (expected != actual)
            {
                return;
            }
            if (expected != null && !expected.Equals(actual)) return;
            message = message ?? $"期望两个值不相等.期望:{expected},实际:{actual}.";
            throw new FactException(message);
        }

        

        public static void Equal(object expected, object actual, string message = null)
        {
            if (expected == null)
            {
                if (actual == null) return;

            }
            else {
                if (expected.Equals(actual)) return;
            }
            message = message ?? $"期望两个值相等.期望:{expected},实际:{actual}.";
            throw new FactException(message);
        }

       

        public static void NotEqual(object expected, object actual, string message = null)
        {
            if (expected == null)
            {
                if (actual != null) return;

            }
            else
            {
                if (!expected.Equals(actual)) return;
            }
            message = message ?? $"期望两个值不相等.期望:{expected},实际:{actual}.";
            throw new FactException(message);
        }

        


        public static void Null(object value,string message =null) {
            if (value == null)
            {
                return;
            }
            message = message ?? $"期望为NULL.实际:{value}.";
            throw new FactException(message);
        }

        


        public static void NotNull(object value, string message = null)
        {
            if (value != null)
            {
                return;
            }
            message = message ?? $"期望不为NULL.实际为空.";
            throw new FactException(message);
        }

        public static void Empty(object value, string message = null)
        {
            if (value == null)
            {
                return;
            }
            var t = value.GetType();
            if (t == typeof(string))
            {
                if ((value as string) == string.Empty) return;
            }
            else if (t.FullName.StartsWith("Nullable`1")) {
                if ((bool)t.GetProperty("HasValue").GetValue(value)==false) return;
            }

            message = message ?? $"期望为空值(NULL/!Nullable.HasValue/空字符串).实际:{value}.";
            throw new FactException(message);
        }

        public static void NotEmpty(object value, string message = null)
        {
            if (value != null)
            {
                var t = value.GetType();
                if (t == typeof(string))
                {
                    if ((value as string) != string.Empty) return;
                }
                else if (t.FullName.StartsWith("Nullable`1"))
                {
                    if ((bool)t.GetProperty("HasValue").GetValue(value) == true) return;
                }
            }
            

            message = message ?? $"期望为空值(NULL/!Nullable.HasValue/空字符串).实际:{value}.";
            throw new FactException(message);
        }

        public static void True(bool value, string message = null)
        {
            if (value ==true)
            {
                return;
            }


            message = message ?? $"期望为TRUE.实际:FALSE.";
            throw new FactException(message);
        }
        public static void False(bool value, string message = null)
        {
            if (value == false)
            {
                return;
            }


            message = message ?? $"期望为FALSE.实际:TRUE.";
            throw new FactException(message);
        }
    }
}
