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
    public class OrderLine : Entity
    {
        [Key]
        public int OrderLineID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

    }
}
