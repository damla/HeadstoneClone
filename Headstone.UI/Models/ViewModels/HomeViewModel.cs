using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ProductListViewModel RecommendedProducts { get; set; }

        public CampaignViewModel Campaign { get; set; }
    }
}