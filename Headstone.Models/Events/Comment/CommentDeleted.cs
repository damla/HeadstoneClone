using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Comment
{
    public class CommentDeleted : BaseEvent
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }
    }
}
