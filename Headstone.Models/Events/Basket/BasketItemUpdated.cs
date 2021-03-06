﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Basket
{
    public class BasketItemUpdated : BaseEvent
    {
        public int BasketItemID { get; set; }

        public int BasketID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        public Headstone.Framework.Models.EntityStatus Status { get; set; }
    }
}