using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class ProductListViewModel : BaseViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}