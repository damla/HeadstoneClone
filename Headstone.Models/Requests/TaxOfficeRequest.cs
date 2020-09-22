using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class TaxOfficeRequest : BaseRequest
    {
        public TaxOfficeRequest()
        {

        }
        public List<int> TaxOfficesIds { get; set; } = new List<int>();

        
    }
}
