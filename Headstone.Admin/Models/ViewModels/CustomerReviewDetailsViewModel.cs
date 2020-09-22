using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.Common;
using Headstone.Models;
using Lidia.Identity.API.Models;
using Headstone.MetaData.API.Models;
using Headstone.MetaData.API.Models.Live;

namespace Headstone.AI.Models.ViewModels
{
    public class CustomerReviewDetailsViewModel : BaseViewModel
    {
        public CustomerReviewViewModel CustomerReview { get; set; }

        public int SourceId { get; set; }

        public string SourceTitle { get; set; }

        public string SourceUserName { get; set; }

        public string Description { get; set; }

        public string Created { get; set; }

        public List<string> Images { get; set; } 

    }
}