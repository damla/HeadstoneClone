using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class TransactionServiceResponse<T> : ServiceResponse<T>
    {
        public int TransactionId { get; set; }

        public int OrderId { get; set; }

    }
}
