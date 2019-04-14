using System;
using System.Collections.Generic;
using System.Text;

namespace CMBPS
{
    public class PageInfo :IPageInfo
    {
        public long PageIndex { get; set; }

        public uint PageSize { get; set; }
    }
}
