using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class CampaignRequest : BaseRequest
    {
        public List<int> CampaignIds { get; set; } = new List<int>();

        public List<DiscountType> CampaignTypes { get; set; } = new List<DiscountType>();

        public List<string> RelatedDataEntityTypes { get; set; } = new List<string>();

        public List<int> RelatedDataEntityIds { get; set; } = new List<int>();

        public List<CampaignProperty> CampaignProperties { get; set; } = new List<CampaignProperty>();
    }
}
