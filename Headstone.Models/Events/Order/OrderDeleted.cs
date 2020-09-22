using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Order
{
    public class OrderDeleted : BaseEvent
    {
        public int OrderId { get; set; }

        public int UserID { get; set; }

        public int? BasketID { get; set; }
    }
}
