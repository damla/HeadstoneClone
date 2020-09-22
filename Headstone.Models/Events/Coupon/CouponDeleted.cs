using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Coupon
{
    public class CouponDeleted : BaseEvent
    {
        public int CouponID { get; set; }
    }
}
