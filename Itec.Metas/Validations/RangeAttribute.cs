using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class RangeAttribute:ValidationAttribute
    {
        public override string Name => "Range";
        public decimal? Max { get; private set; }
        public decimal? Min { get; private set; }
        public RangeAttribute(decimal? min, decimal? max = null)
        {
            this.Max = max;
            this.Min = min;
        }

        public override string Check(object value)
        {
            if (value == null) return null;
            
            if (this.Min != null && (decimal)value < this.Min.Value)
            {
                return "Min";
            }
            if (this.Max != null && (decimal)value > this.Max.Value)
            {
                return "Max";
            }
            return string.Empty;
        }

    }
}
