using Headstone.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class UserAddressViewModel : BaseViewModel
    {
        public int AddressId { get; set; }

        public string AddressName { get; set; }

        [Display(Name = "Açık Adres")]
        public string StreetAddress { get; set; }

        [Display(Name = "Mahalle")]
        public string Neighborhood { get; set; }

        [Display(Name = "İlçe")]
        public string District { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        [Display(Name = "Ülke")]
        public string Country { get; set; }

        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Display(Name = "Posta Kodu")]
        public string ZipCode { get; set; }

        [Display(Name = "Adres Başlığı")]
        public string Title { get; set; }

        [Display(Name = "Adres Adı")]
        public string Name { get; set; }

        public int OrganizationId { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "İl")]
        public List<GeoLocation> Cities { get; set; }

        [Display(Name = "Vergi Dairesi")]
        public List<TaxOffice> TaxOffices { get; set; }

    }
}