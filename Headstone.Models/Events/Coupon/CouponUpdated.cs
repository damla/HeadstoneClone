using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Coupon
{
    public class CouponUpdated : BaseEvent
    {
        public int CouponID { get; set; }

        public string CouponCode { get; set; }

        public int? OwnerID { get; set; }

        public int? CampaignID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Headstone.Framework.Models.EntityStatus Status { get; set; }
    }
}
