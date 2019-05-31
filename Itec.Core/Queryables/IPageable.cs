using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface IPageable<T> :IFilterable<T>
    {
        List<T> Items { get; }

        long RecordCount { get;  }

        int PageSize { get; set; }

        int PageIndex { get; set; }

        int PageCount { get; }
    }
}
