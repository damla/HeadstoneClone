using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class OrderRequest : BaseRequest
    {
        public List<int> OrderIds { get; set; } = new List<int>();

        public List<int> UserIds { get; set; } = new List<int>();

    }
}
