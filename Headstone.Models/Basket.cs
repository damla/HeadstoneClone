using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class Basket : Entity
    {
        [Key]
        public int BasketID { get; set; }

        public int UserID { get; set; }

        #region [ Navigation Properties ]

        public virtual List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

        #endregion

    }
}
