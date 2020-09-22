using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class TicketListViewModel : BaseViewModel
    {
        public List<TicketViewModel> Tickets { get; set; } = new List<TicketViewModel>();

    }
}