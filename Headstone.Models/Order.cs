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
    public class Order : Entity
    {
        [Key]
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public int BasketID { get; set; }

        public int CampaignID { get; set; }

        public int AddressID { get; set; }

        public int BillingInfoID { get; set; }

        public decimal TotalPrice { get; set; }

        #region [ Navigation Properties ]

        [ForeignKey("BasketID")]
        public Basket Basket { get; set; }

        [ForeignKey("CampaignID")]
        public Campaign Campaign { get; set; }

        public virtual List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        public virtual List<OrderProperty> OrderProperties { get; set; } = new List<OrderProperty>();


        #endregion
    }
}
