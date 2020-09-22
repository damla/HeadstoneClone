using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.RequestModels
{
    public class DataQueryRequest
    {
        public int draw { get; set; }

        public int start { get; set; }

        public int length { get; set; }

        public string search { get; set; }

        //public string search { get; set; }
    }
}