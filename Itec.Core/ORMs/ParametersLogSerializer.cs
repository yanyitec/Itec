using Itec.Logs;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Itec.ORMs
{
    public class ParametersLogSerializer : Itec.Logs.ILogSerializable
    {
        public ParametersLogSerializer(System.Data.Common.DbParameterCollection coll) {
            this.Parameters = coll;
        }
        public DbParameterCollection Parameters { get; private set; }
        public string Serialize(ILogWriter details)
        {
            var sb = new StringBuilder();
            foreach (DbParameter par in this.Parameters) {
                sb.Append(par.ParameterName).Append(": ").Append(par.Value==DBNull.Value?"<NULL>":par.Value.ToString());
            }
            return sb.ToString();
        }
    }
}
