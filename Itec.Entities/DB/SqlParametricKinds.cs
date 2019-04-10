using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities.DB
{
    /// <summary>
    /// sql参数化类型
    /// </summary>
    public enum SqlParametricKinds
    {
        /// <summary>
        /// @
        /// </summary>
        At,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        Value
    }
}
