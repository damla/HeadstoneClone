using Headstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class OrderDetailViewModel : BaseViewModel
    {
        public Order Order { get; set; }

        public UserViewModel User { get; internal set; }
    }
}