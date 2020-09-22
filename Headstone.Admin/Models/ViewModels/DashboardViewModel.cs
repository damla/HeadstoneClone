using System.Collections.Generic;

namespace Headstone.AI.Models.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public List<ChartViewModel> Chart { get; set; } = new List<ChartViewModel>();

        public int OrderCount { get; set; }

        public decimal OrganizationsWithAnOrder { get; set; }

        public decimal? OrderTotal { get; set; }

        public decimal? OrderAverage { get; set; }

        public int ProductQuantity { get; set; }

        public int RefundTotal { get; set; }

        public int CancelTotal { get; set; } = 0;

        public int OrganizationCount { get; set; }

        public decimal OrganizationOwnerPercentage { get; set; }

        public int UserCount { get; set; }

        public int TenderCount { get; set; }

        public int OfferCount { get; set; }


    }
}