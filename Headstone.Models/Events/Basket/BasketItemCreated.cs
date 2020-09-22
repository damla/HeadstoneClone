using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Basket
{
    public class BasketItemCreated : BaseEvent
    {
        public int BasketID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
