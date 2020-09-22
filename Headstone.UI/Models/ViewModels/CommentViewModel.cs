using Lidia.Identity.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class CommentViewModel : BaseViewModel
    {
        public string Comment { get; set; }

        public int Rating { get; set; }

        public User Commenter { get; set; }
    }
}