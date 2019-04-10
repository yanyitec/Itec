using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface IValidation
    {
        /// <summary>
        /// 检查值是否符合要求
        /// </summary>
        /// <param name="value"></param>

        /// <returns>
        /// null : 未检查，比如检查是否为数字，但输入没有
        /// Empty:通过
        /// string:错误码
        /// </returns>
        string Check(object value);

        string Name { get; }
    }
}
