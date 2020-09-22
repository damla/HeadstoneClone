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
    public class GeoLocation : Entity
    {
        [Key]
        public int GeoLocationId { get; set; }

        public string Path { get; set; }

        public string Code { get; set; }

        public string Parent { get; set; }

        public string Name { get; set; }

        public GeoLocaitonTypes Type { get; set; }

        public int SortOrder { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }
    }

}
