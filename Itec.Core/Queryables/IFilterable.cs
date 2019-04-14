using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Itec
{
    public interface IFilterable<T>
    {
        Expression<Func<T, object>> AscendingExpression { get; set; }
        Expression<Func<T, object>> DescendingExpression { get; set; }
        Expression<Func<T, bool>> QueryExpression { get; set; }
        ParameterExpression QueryParameter { get; }


        int SkipCount { get; set; }
        int TakeCount { get; set; }

        

        //IList<T> ToList();
    }
}