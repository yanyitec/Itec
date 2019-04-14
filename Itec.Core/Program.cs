using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Metas
{
    public static class Program
    {
        public static void Main(string[] args) {
            //Metas.Tests.MetaTest.Run(args);
            var ormTest = new Tests.ORMTest();
            ormTest.CreateTable();
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}
