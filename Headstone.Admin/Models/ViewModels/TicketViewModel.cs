using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class TicketViewModel : BaseViewModel
    {
        public int TicketId { get; internal set; }

        //public TicketType Type { get; internal set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}