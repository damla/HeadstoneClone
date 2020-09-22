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
    public class Transaction : Entity
    {
        [Key]
        public int TransactionID { get; set; }

        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int OrderID { get; set; }

        public string CardNumber { get; set; }

        public decimal TotalPrice { get; set; }


        #region [ Navigation Properties ]

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        #endregion
    }
}
