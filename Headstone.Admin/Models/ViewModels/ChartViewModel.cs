using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class ChartViewModel
    {
        public string Date { get; set; }

        public double OrderTotal { get; set; }

        public int OrderCount { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public int OrganizationId { get; set; }
    }
}