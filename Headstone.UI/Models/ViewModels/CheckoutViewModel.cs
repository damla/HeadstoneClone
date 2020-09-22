using Lidia.Identity.API.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class CheckoutViewModel : BaseViewModel
    {
        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string CVV2 { get; set; }

        public string CardExpiryMonth { get; set; }

        public string CardExpiryYear { get; set; }

        public int AddressId { get; set; }

        public string StreetAddress { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Country { get; set; }

        public string AddressName { get; set; }

        public string TotalPrice { get; set; }

        public OrderViewModel Order { get; set; }

        public UserAddress Address { get; set; }

        public UserBillingInfo BillingInfo { get; set; }

    }
}