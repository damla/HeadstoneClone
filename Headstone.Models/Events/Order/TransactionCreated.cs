using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Order
{
    public class TransactionCreated : BaseEvent
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int OrderID { get; set; }

        public string CardNumber { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
