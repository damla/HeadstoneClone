using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class GeoLocationRequest : BaseRequest
    {
        public GeoLocationRequest()
        {

        }

        public string Prefix { get; set; }

        public List<int> GeoLocationIds { get; set; } = new List<int>();

        public List<string> GeoLocationPaths { get; set; } = new List<string>();

        public List<string> GeoLocationParents { get; set; } = new List<string>();

    }
}
