using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events
{
    public class PictureEvent : BaseEvent
    {
        public int PictureId { get; set; }

        public string PictureKey { get; set; }

        public string ImageUrl { get; set; }

        public string Alt { get; set; }

        public int SortOrder { get; set; }

    }
}
