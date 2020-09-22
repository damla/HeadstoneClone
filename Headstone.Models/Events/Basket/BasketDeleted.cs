using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Basket
{
    public class BasketDeleted : BaseEvent
    {
        public int BasketID { get; set; }
    }
}
