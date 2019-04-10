using System;

namespace Itec.Entities
{
    class Program
    {
        // var model = Model.LoadConfig(config).SetFields("a,b");
        // model.
        // model.validate();
        //
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var sqlTester = new Tests.SqlTest();
            sqlTester.CreateTable();
        }
    }
}
