using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities.Tests
{
    public class Article:RecordEntity
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
