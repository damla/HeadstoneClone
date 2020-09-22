using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public int Id { get; set; }

        public string Password { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public string Gender { get; set; }

    }
}