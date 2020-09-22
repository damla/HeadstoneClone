using Headstone.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class CommentRequest :BaseRequest
    {
        public List<int> CommentIds { get; set; } = new List<int>();

        public List<CommentType> CommentTypes { get; set; } = new List<CommentType>();

        public List<string> RelatedDataEntityTypes { get; set; } = new List<string>();

        public List<int> RelatedDataEntityIds { get; set; } = new List<int>();

        public List<CommentProperty> CommentProperties { get; set; } = new List<CommentProperty>();

        public List<CommentTag> CommentTags { get; set; } = new List<CommentTag>();

    }
}