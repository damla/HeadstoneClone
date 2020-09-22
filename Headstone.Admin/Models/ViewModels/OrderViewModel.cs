using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; internal set; }
        public string OrderCode { get; internal set; }
        public decimal Total { get; internal set; }
        public string Currency { get; internal set; }
        public string RefNumber { get; internal set; }
        public DateTime Created { get; internal set; }
        public string FullName { get; internal set; }
        public string Email { get; internal set; }
        public string UserName { get; internal set; }
        public int Quantity { get; internal set; }
    }
}