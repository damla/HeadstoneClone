using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class TransactionRequest : BaseRequest
    {
        public List<int> TransactionIds { get; set; } = new List<int>();

        public List<int> UserIds { get; set; } = new List<int>();

        public List<int> OrderIds { get; set; } = new List<int>();

    }
}
