using Headstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class BasketItemViewModel : BaseViewModel
    {
        public int BasketItemID { get; set; }

        public int BasketID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public string Price { get; set; }

        public string ListPrice { get; set; }

        public string OldPrice { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalListPrice
        {
            get
            {
                return Convert.ToDecimal(this.ListPrice) * this.Quantity;
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return Convert.ToInt32(this.Price) * this.Quantity;
            }
        }

        public decimal TotalOriginalPrice
        {
            get
            {
                return Convert.ToInt32(this.OldPrice) * this.Quantity;
            }
        }

        public ProductViewModel Product { get; set; }

        public BasketItemViewModel From(BasketItem item)
        {
            this.BasketItemID = item.BasketItemID;
            this.BasketID = item.BasketID;
            this.ProductID = item.ProductID;
            this.Quantity = item.Quantity;
            this.BasePrice = item.BasePrice;

            return this;
        }
    }
}