using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Comment
{
    public class CommentPropertyDeleted : PropertyEvent
    {
        public int PropertyId { get; set; }

        public int CommentId { get; set; }

    }
}
