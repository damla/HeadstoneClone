using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Campaign
{
    public class CampaignPropertyDeleted : PropertyEvent
    {
        public int PropertyId { get; set; }

        public int CampaignID { get; set; }

    }
}
