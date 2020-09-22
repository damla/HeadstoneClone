using Headstone.Common;
using Headstone.Framework.Models;
using System.ComponentModel.DataAnnotations;


namespace Headstone.Models
{
    public class Tag : Entity
    {
        [Key]
        public int TagId { get; set; }

        public TagType Type { get; set; }

        public string Culture { get; set; }

        public string Value { get; set; }

    }
}
