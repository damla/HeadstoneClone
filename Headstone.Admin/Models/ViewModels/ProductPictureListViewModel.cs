using Headstone.Models;
using Headstone.MetaData.API.Models.Live;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Models.ViewModels
{
    public class ProductPictureListViewModel : BaseViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Ürün resimleri")]
        public List<ProductPicture> Pictures { get; set; }

        public string PictureKey { get; set; }

        public bool isActiveProduct { get; set; }
    }
}
