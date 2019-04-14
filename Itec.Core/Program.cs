using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Metas
{
    public static class Program
    {
        public static void Main(string[] args) {
            //Metas.Tests.MetaTest.Run(args);
            var ormTest = new Tests.ORMTest();
            var task =Task.Run(async ()=> await ormTest.CreateTableAsync());
            //task.RunSynchronously();
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}
