using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class TrademarkListViewModel : BaseViewModel
    {
        public List<TrademarkViewModel> Trademarks { get; set; } = new List<TrademarkViewModel>();

    }
}