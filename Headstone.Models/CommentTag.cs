using System.ComponentModel.DataAnnotations.Schema;


namespace Headstone.Models
{
    public class CommentTag : Tag
    {
        public int CommentId { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }

        #endregion
    }
}
