using System.Collections.Generic;

namespace Headstone.UI.Models.ViewModels
{
    public class HeaderViewModel : BaseViewModel
    {
        public List<BreadcrumbItemViewModel> Breadcrumb { get; set; } = new List<BreadcrumbItemViewModel>();

        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        public int FavItemCount { get; set; }
    }
}