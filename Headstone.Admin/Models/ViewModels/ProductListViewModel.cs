using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class ProductListViewModel : BaseViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }
}