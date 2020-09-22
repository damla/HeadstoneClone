using Headstone.Framework.Models;
using Lidia.Identity.API.Models;
using System;
using System.Collections.Generic;

namespace Headstone.UI.Models.ViewModels
{
    public class BaseViewModel
    {
        public User CurrentUser { get; set; }

        public string CurrentCulture { get; set; }

        public string SiteRoot { get; set; }

        public EntityStatus Status { get; set; }

        public List<string> Errors = new List<string>();

        public List<string> Result = new List<string>();

        public DateTime Created { get; set; }

        public string SuccessMessage { get; set; }

    }
}