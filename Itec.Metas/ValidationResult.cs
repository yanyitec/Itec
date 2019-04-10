using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public class ValidationResult
    {
        public ValidationResult(IValidation validation, string errorCode) {
            this.Name = validation.Name;
            this.ErrorCode = errorCode;
            this.Validation = validation;
        }
        public string Name { get; set; }
        public string ErrorCode { get; set; }
        public IValidation Validation { get; set; }
    }
}
