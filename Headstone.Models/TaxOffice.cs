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
    public class TaxOffice : Entity
    {
        [Key]
        public int TaxOfficeId { get; set; }

        public string Name { get; set; }

        public string GeoCode { get; set; }

        public string Code { get; set; }
    }
}
