using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class UserListViewModel : BaseViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

    }
}