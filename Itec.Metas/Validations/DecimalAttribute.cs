using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class DecimalAttribute:ValidationAttribute
    {
        public override string Name => "Decimal";
        public int? Integer { get; private set; }
        public int? Scale { get; private set; }
        public DecimalAttribute(int? integer, int? scale = null)
        {
            this.Integer = integer;
            this.Scale = scale;
        }

        public override string Check(object value)
        {
            if (value == null) return null;
            var intCount = 0;
            var floatCount = 0;
            var c = 0;
            var meetDot = false;
            var val = value.ToString();
            foreach (char ch in val)
            {
                if (ch == ' ' || ch == '\n' || ch == '\r' || ch == '\t' || ch == ',') continue;
                if (ch == '.')
                {
                    intCount = c;
                    c = 0;
                    meetDot = true;
                    continue;
                }
                if (ch <= '0' || ch >= '9') return "Decimal";
                c++;
            }
            if (meetDot) floatCount = c;
            
            if (Scale!=null && floatCount>Scale.Value)
            {
                return "scale";
            }
            if (Integer != null && intCount > Integer.Value)
            {
                return "integer";
            }
            return string.Empty;
        }
    }
}
