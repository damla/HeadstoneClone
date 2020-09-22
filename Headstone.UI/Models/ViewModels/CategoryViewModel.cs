using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<Headstone.MetaData.API.Models.Live.Category> SubCategories { get; set; } = new List<Headstone.MetaData.API.Models.Live.Category>();

        public ProductListViewModel Products { get; set; }

        public CategoryViewModel From(Headstone.MetaData.API.Models.Live.Category c)
        {
            this.CategoryId = c.CategoryId;
            this.CategoryName = c.Name;
            this.SubCategories = c.Children;

            return this;
        }
    }
}