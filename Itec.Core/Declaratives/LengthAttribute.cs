using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Declaratives
{
    public class LengthAttribute:ValidationAttribute
    {
        public override string Name => "Length";
        public int? Max { get; private set; }
        public int? Min { get; private set; }
        public LengthAttribute(int? min, int? max = null) {
            this.Max = max;
            this.Min = min;
        }

        public override string Check(object value)
        {
            if (value == null) return null;
            var val = value.ToString();
            if (this.Min!=null && val.Length < this.Min.Value) {
                return   "Min" ;
            }
            if (this.Max != null && val.Length > this.Max.Value)
            {
                return  "Max" ;
            }
            return string.Empty;
        }

    }
}
