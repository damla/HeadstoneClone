using Headstone.Common;
using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class Comment : Entity
    {
        [Key]
        public int CommentId { get; set; }

        public int? ParentId { get; set; }

        public int UserId { get; set; }

        public string RelatedDataEntityType { get; set; }

        public int RelatedDataEntityId { get; set; }

        public CommentType Type { get; set; }

        public short? Rating { get; set; }

        public string Body { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("ParentId")]
        public virtual Comment Parent { get; set; }

        public virtual List<CommentProperty> Properties { get; set; }

        public virtual List<CommentTag> Tags { get; set; }

        #endregion

    }
}
