using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    /// <summary>
    /// 放在属性上面的验证器的抽象基类
    /// </summary>
    public abstract class ValidationAttribute:Attribute,IValidation
    {
        public abstract string Name { get; }
        /// <summary>
        /// 检查值是否符合要求
        /// </summary>
        /// <param name="value"></param>
        
        /// <returns>
        /// null : 未检查，比如检查是否为数字，但输入没有
        /// Empty:通过
        /// string:错误码
        /// </returns>
        public abstract string Check(object value);
    }
}
