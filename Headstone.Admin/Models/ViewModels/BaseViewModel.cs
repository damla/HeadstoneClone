using Lidia.Identity.API.Models;
using System.Collections.Generic;

namespace Headstone.AI.Models.ViewModels
{
    public class BaseViewModel
    {
        public User CurrentUser { get; set; }

        public string CurrentCulture { get; set; }

        public List<string> Errors  = new List<string>();

        public List<string> Result = new List<string>();
    }
}