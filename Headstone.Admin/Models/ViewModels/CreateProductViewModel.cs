using Headstone.MetaData.API.Models.Live;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Models.ViewModels
{
    public class CreateProductViewModel : BaseViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Ürün Adı*"), Required]
        public string Name { get; set; }

        [Display(Name="EAN Kodu")]
        public string EANCode { get; set; }

        [Display(Name = "Ürün Kodu*"), Required]
        public string Code { get; set; }

        [Required,Display(Name = "KDV Oranı")]
        public decimal TaxRate { get; set; }

        [Required, Display(Name = "Fiyat")]
        public decimal Price { get; set; }

        public decimal ListPrice { get; set; }


        [Required, Display(Name = "Stok")]
        public int Stock { get; set; }

        [Required]
        public string Trademark { get; set; }

        public string TempPictureKey { get; set; }

        public string Category { get; set; }

        public List<string> CategoryIds { get; set; }

        [Display(Name = "Ürün Markası*")]
        public List<Trademark> Trademarks { get; set; }

        [Display(Name = "Ürün Görselleri")]
        public List<ProductPicture> Pictures { get; set; }

        [Display(Name = "Ürün Kategorisi*")]
        public List<CategoryViewModel> Categories { get; set; }

        public bool IsPartial { get; set; }

        public bool IsEdit { get; set; }

        public bool IsActive { get; set; }

        public int SelectedTrademarkId { get; set; }

        public List<int> SelectedCategoryIds { get; set; }
    }
}
