using Headstone.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class UserBillingInfoViewModel : BaseViewModel
    {
        public int BillingInfoId { get; set; }

        public string SourceBillingInfoCode { get; set; }

        public int UserId { get; set; }

        [Display(Name="Adres Başlığı")]
        public string Title { get; set; }

        [Display(Name = "Adres Adı")]
        public string Name { get; set; }

        [Display(Name = "Açık Adres")]
        public string StreetAddress { get; set; }

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

        [Display(Name = "Vergi Numarası")]
        public string TaxNumber { get; set; }

        public string TaxOffice { get; set; }

        [Display(Name = "İl")]
        public List<GeoLocation> Cities { get; set; }

        [Display(Name = "Vergi Dairesi")]
        public List<TaxOffice> TaxOffices { get; set; }

        public int OrganizationId { get; set; }


    }
}