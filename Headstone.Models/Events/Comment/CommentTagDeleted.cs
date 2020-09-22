using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Comment
{
    public class CommentTagDeleted : TagEvent
    {
        public int TagId { get; set; }

        public int CommentId { get; set; }

    }
}
