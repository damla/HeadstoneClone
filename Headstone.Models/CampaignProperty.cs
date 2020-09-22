using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class CampaignProperty : Property
    {
        public int CampaignID { get; set; }

        #region [ Navigation Properties ]

        [ForeignKey("CampaignID")]
        public virtual Campaign Campaign { get; set; }

        #endregion
    }
}
