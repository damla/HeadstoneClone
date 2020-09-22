using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class StatsRequest : BaseRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool ReturnDailyTrend { get; set; } = false;
    }
}
