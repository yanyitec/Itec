using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Metas
{
    public static class Program
    {
        public static void Main(string[] args) {
            //Metas.Tests.MetaTest.Run(args);
            string[] includes = null; 
                //= new string[] { };
            Fact.Test(includes);
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}
