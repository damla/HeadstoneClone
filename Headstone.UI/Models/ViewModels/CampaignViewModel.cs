using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class CampaignViewModel
    {
        public DiscountType DiscountType { get; set; }

        public ProductViewModel Product { get; set; }

        public int DiscountAmount { get; set; }

        public string Discountmessage
        {
            get
            {
                if (this.DiscountType == DiscountType.Amount)
                {
                    return DiscountAmount + "$ indirim!";
                }
                else
                {
                    return "% " + DiscountAmount + " indirim!";
                }
            }
        }


    }
}