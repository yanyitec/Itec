using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Itec.ORMs
{
    public class DbClassFactory : MetaFactory
    {
        public new  static DbClassFactory Default = new DbClassFactory();

        protected override Type MetaClassType => typeof(DbClass<>);

    }
}
