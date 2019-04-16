using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORMs.SQLs
{
    public class WhereOpts
    {
        public IDbProperty LastProp { get; set; }
        public bool ValueEmbeded { get; set; }
        public int NoSeed { get; set; }
    }
}
