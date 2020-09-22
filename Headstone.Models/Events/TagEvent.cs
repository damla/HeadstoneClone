using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events
{
    public class TagEvent : BaseEvent
    {
        public TagType Type { get; set; }

        public string Culture { get; set; }

        public string Value { get; set; }


    }
}
