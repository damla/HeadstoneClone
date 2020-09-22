using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class CampaignServiceResponse<T> : ServiceResponse<T>
    {
        public int CampaignID { get; set; }

        public int PropertyId { get; set; }
    }
}
