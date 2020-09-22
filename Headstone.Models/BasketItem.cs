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
    public class BasketItem : Entity
    {
        [Key]
        public int BasketItemID { get; set; }

        public int BasketID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        #region [ Navigation Properties ]

        [ForeignKey("BasketID")]
        public Basket Basket { get; set; }

        #endregion

    }
}
