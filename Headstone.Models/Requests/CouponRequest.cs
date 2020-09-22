using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class CouponRequest : BaseRequest
    {
        public List<int> CouponIDs { get; set; } = new List<int>();

        public List<int?> OwnerIDs { get; set; } = new List<int?>();

        public List<int?> CampaignIDs { get; set; } = new List<int?>();
    }
}
