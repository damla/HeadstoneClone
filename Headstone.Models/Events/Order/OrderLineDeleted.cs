﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Order
{
    public class OrderLineDeleted : BaseEvent
    {
        public int OrderLineID { get; set; }
    }
}
