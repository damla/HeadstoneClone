using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Order
{
    public class OrderCreated : BaseEvent
    {
        public int UserID { get; set; }

        public int? BasketID { get; set; }

        public int CampaignID { get; set; }

        public int AddressID { get; set; }

        public int BillingInfoID { get; set; }

        public decimal TotalPrice { get; set; }

        #region [ Navigation Properties ]

        public virtual List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        public virtual List<OrderProperty> OrderProperties { get; set; } = new List<OrderProperty>();

        #endregion  
    }
}
