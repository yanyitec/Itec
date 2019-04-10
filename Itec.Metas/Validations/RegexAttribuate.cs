using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class RegexAttribuate:ValidationAttribute
    {
        public override string Name => "Regex";
        public RegexAttribuate(string pattern) {
            this.Regex = new System.Text.RegularExpressions.Regex(pattern);
            this.Pattern = pattern;
        }
        public string Pattern { get; private set; }

        public System.Text.RegularExpressions.Regex Regex { get; private set; }

        public override string Check(object value)
        {
            if (value == null) return null;
            var txt = value.ToString();
            return this.Regex.IsMatch(txt)?string.Empty:"Regex";
        }
    }
}
