using Headstone.Models;
using Lidia.Identity.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public int BasketID { get; set; }

        public int CampaignID { get; set; }

        public int AddressID { get; set; }

        public int BillingInfoID { get; set; }

        public decimal TotalPrice { get; set; }

        public BasketViewModel Basket { get; set; }

        public List<UserAddress> Addresses { get; set; } = new List<UserAddress>();

        public List<UserBillingInfo> BillingInfos { get; set; } = new List<UserBillingInfo>();

        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();

        public List<OrderProperty> Properties { get; set; } = new List<OrderProperty>();

        public OrderViewModel From(Order order)
        {
            this.OrderID = order.OrderID;
            this.UserID = order.UserID;
            this.BasketID = order.BasketID;
            this.CampaignID = order.CampaignID;
            this.AddressID = order.AddressID;
            this.BillingInfoID = order.BillingInfoID;
            this.TotalPrice = order.TotalPrice;
            this.Lines = order.OrderLines;
            this.Properties = order.OrderProperties;

            return this;
        }
    }
}