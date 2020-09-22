using Headstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class BasketViewModel : BaseViewModel
    {
        public int BasketId { get; set; }

        public int UserId { get; set; }

        public int TotalListPrice
        {
            get
            {
                return this.BasketItems.Select(p => Convert.ToInt32(p.TotalListPrice)).Sum();
            }
        }

        public int TotalPrice
        {
            get
            {
                return this.BasketItems.Select(p => Convert.ToInt32(p.TotalPrice)).Sum();
            }
        }

        public int TotalOldPrice
        {
            get
            {
                return this.BasketItems.Select(p => Convert.ToInt32(p.TotalOriginalPrice)).Sum();
            }
        }

        public virtual List<BasketItemViewModel> BasketItems { get; set; } = new List<BasketItemViewModel>();

        public BasketViewModel From(Basket b)
        {
            this.BasketId = b.BasketID;
            this.UserId = b.UserID;
            this.BasketItems = b.BasketItems.Select(item => new BasketItemViewModel().From(item)).ToList();

            return this;
        }
    }

}