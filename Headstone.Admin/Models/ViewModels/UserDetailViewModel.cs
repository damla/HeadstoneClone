using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lidia.Identity.API.Models;

namespace Headstone.AI.Models.ViewModels
{
    public class UserDetailViewModel
    {
        public UserViewModel UserModel { get; set; }

        public List<RoleViewModel> Roles { get; set; }

        public List<UserAddressViewModel> Addresses { get; set; }

        public List<UserBillingInfoViewModel> BillingInfos { get; set; }

        public List<UserConsentViewModel> Consents { get; set; }

        public List<OrderViewModel> Orders { get; set; }

        public List<TransactionViewModel> Transactions { get; set; }
    }
}