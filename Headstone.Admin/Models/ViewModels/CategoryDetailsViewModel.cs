using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Headstone.MetaData.API.Models.Live;
using Headstone.MetaData.Common.Models;

namespace Headstone.AI.Models.ViewModels
{
    public class CategoryDetailsViewModel : BaseViewModel
    {
        public CategoryViewModel Category { get; set; }

        public List<Product> Products { get; set; }
    }
}