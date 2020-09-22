using Headstone.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models
{
    public class OrderProperty : Property
    {
        public int OrderID { get; set; }

        #region [ Navigation properties ]

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        #endregion
    }
}
