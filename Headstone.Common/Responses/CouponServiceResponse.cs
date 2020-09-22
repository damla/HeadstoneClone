using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class CouponServiceResponse<T> : ServiceResponse<T>
    {
        public int CouponID { get; set; }
    }
}
