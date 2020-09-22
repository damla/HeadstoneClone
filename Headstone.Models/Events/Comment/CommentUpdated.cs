using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Events.Comment
{
    public class CommentUpdated : BaseEvent
    {
        public int CommentId { get; set; }

        public int? ParentId { get; set; }

        public int UserId { get; set; }

        public string RelatedDataEntityType { get; set; }

        public int RelatedDataEntityId { get; set; }

        public CommentType Type { get; set; }

        public short? Rating { get; set; }

        public string Body { get; set; }

        public Headstone.Framework.Models.EntityStatus Status { get; set; }
    }
}
