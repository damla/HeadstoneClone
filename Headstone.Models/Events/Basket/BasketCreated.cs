using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Basket
{
    public class BasketCreated : BaseEvent
    {
        public int UserID { get; set; }

        public List<BasketItemCreated> BasketItems { get; set; } = new List<BasketItemCreated>();
    }
}