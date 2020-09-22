using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class OrderLineRequest : BaseRequest
    {
        public List<int> OrderIds { get; set; } = new List<int>();

        public List<int> OrderLineIds { get; set; } = new List<int>();

    }
}
