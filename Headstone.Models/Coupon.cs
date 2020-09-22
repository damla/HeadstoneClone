using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class Coupon : Entity
    {
        [Key]
        public int CouponID { get; set; }

        public string CouponCode { get; set; }

        public int? OwnerID { get; set; }

        public int? CampaignID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        #region [ Navigation Properties ]

        [ForeignKey("CampaignID")]
        public virtual Campaign Campaign { get; set; }

        #endregion

    }
}
