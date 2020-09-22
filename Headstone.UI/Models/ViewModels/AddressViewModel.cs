using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class AddressViewModel : BaseViewModel
    {
        public int AddressID { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string District { get; set; }

        public string Name { get; set; }

        public string StreetAddress { get; set; }

        public int UserID{ get; set; }

        public string ZipCode{ get; set; }


    }
}