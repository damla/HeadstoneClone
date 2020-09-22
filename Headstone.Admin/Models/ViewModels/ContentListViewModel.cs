using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class ContenListViewModel : BaseViewModel
    {
        public List<ContentViewModel> Contents { get; set; } = new List<ContentViewModel>();
    }
}