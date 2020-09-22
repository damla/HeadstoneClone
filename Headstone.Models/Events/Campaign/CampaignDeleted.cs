using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Campaign
{
    public class CampaignDeleted : BaseEvent
    {
        public int CampaignID { get; set; }
    }
}
