using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class OrderServiceResponse<T> : ServiceResponse<T>
    {
        public int OrderId { get; set; }

        public int OrderLineId { get; set; }

    }
}
