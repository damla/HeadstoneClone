using Headstone.Common;
using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class Campaign : Entity
    {
        [Key]
        public int CampaignID { get; set; }

        public string RelatedDataEntityName { get; set; }

        public int RelatedDataEntityID { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public DiscountType DiscountType { get; set; }

        public decimal DiscountAmount { get; set; }

        #region [ Navigation Properties ]

        public List<CampaignProperty> CampaignProperties { get; set; } = new List<CampaignProperty>();

        #endregion

    }
}
