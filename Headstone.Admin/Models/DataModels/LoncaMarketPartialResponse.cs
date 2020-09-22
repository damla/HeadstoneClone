using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.Responses
{
    public class headstonePartialResponse
    {
        public int ReturnCode { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string Html { get; set; }
    }
}