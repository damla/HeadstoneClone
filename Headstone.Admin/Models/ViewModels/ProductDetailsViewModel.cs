using Lidia.Framework.Models;
using Lidia.MetaData.API.Models.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilkomPazaryeri.AI.Models.ViewModels
{
    public class ProductDetailsViewModel : BaseViewModel
    {
        public Product Product { get; set; }

        public List<Category> Categories { get; set; }

        public List<Trademark> Trademarks { get; set; }

        public string OrganizationName { get; set; }

    }
}