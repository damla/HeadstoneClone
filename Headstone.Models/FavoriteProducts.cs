using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class FavoriteProducts : Entity
    {
        [Key]
        public int FavId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }
    }
}
