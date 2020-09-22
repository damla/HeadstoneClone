using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events
{
    public class PropertyEvent : BaseEvent
    {
        public string Key { get; set; }

        public string Culture { get; set; }

        public string Value { get; set; }

        public string Extra { get; set; }

    }
}
