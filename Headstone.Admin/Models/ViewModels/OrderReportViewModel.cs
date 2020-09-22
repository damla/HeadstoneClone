using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class OrderReportViewModel : BaseViewModel
    {
        public int OrderId { get; set; }
      
        public string ProductName { get; set; }

        public DateTime Created { get; set; }

        public string Trademark { get; set; }

        public string Categories { get; set; }

        public string ResellerName { get; set; }

        public string UnitPrice { get; set; }

        public string Price { get; set; }

        public string Quantity { get; set; }

        public string TotalPrice { get; set; }

        public string Fullname { get; set; }

        public string DeliveryAddress { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string OrderChannel { get; set; }

        public string OrderChannelResponse { get; set; }

        public string PosAccessCode { get; set; }
    }
}