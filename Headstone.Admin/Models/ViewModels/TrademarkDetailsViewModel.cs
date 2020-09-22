using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Headstone.MetaData.API.Models.Live;

namespace Headstone.AI.Models.ViewModels
{
    public class TrademarkDetailsViewModel : BaseViewModel
    {
        public TrademarkViewModel Trademark { get; set; }

        public List<Product> Products { get;  set; }
    }
}