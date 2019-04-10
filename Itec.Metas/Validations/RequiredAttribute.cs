using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class RequiredAttribute:ValidationAttribute
    {
        public override string Name => "Required";
        public RequiredAttribute(bool trim = false) {
            this.Trim = trim;
        }
        public bool Trim { get; set; }

        public override string Check(object value)
        {
            if (value == null) return "Required";
            if (Trim) {
                var val = value.ToString().Trim();
                return val == string.Empty ? "Required" : string.Empty;
            }
            var en = value as System.Collections.IEnumerable;
            if (en != null) {
                foreach (var item in en) {
                    return string.Empty;
                }
                return "Required";
            }
            return value.IsNull()?"Required":string.Empty;
        }
    }
}
