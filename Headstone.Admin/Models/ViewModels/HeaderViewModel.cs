using System.Collections.Generic;

namespace Headstone.AI.Models.ViewModels
{
    public class HeaderViewModel : BaseViewModel
    {
        public List<BreadcrumbItemViewModel> Breadcrumb { get; set; } = new List<BreadcrumbItemViewModel>();
    }
}