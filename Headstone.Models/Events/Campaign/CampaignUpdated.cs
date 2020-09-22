using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Campaign
{
    public class CampaignUpdated : BaseEvent
    {
        public int CampaignID { get; set; }

        public string RelatedDataEntityName { get; set; }

        public int RelatedDataEntityID { get; set; }
        
        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public DiscountType DiscountType { get; set; }

        public decimal DiscountAmount { get; set; }

        public Headstone.Framework.Models.EntityStatus Status { get; set; }
    }
}
