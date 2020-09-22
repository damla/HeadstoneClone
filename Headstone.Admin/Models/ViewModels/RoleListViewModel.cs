using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class RoleListViewModel : BaseViewModel
    {
        public List<RoleViewModel> Trademarks { get; set; } = new List<RoleViewModel>();

    }
}