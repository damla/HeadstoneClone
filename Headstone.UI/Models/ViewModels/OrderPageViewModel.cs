using System.Collections.Generic;

namespace Headstone.UI.Models.ViewModels
{
    public class OrderPageViewModel : BaseViewModel
    {
        public List<OrderViewModel> ItemsOrder { get; set; } = new List<OrderViewModel>();

        public List<ProductViewModel> ItemsProducts { get; set; } = new List<ProductViewModel>();
    }
}