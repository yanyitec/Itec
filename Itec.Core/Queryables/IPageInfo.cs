using System;
using System.Collections.Generic;
using System.Text;

namespace CMBPS
{
    public interface IPageInfo
    {
        long PageIndex { get; set; }

        uint PageSize { get; set; }
    }
}
