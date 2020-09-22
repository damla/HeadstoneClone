using Headstone.MetaData.API.Models.Live;
using Headstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.UI.Models.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ListPrice { get; set; }

        public string Price { get; set; }

        public string ShortDescription { get; set; }

        public Category Category { get; set; }

        public int AverageRating
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.Comments.Select(r => r.Rating).Average());
                }

                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

        public ProductViewModel From(Product p)
        {
            this.ProductId = p.ProductId;
            this.Name = p.Name;
            this.ShortDescription = p.ShortDescription;
            this.Price = p.Properties.FirstOrDefault(k => k.Key == "Price").Value;
            this.ListPrice = p.Properties.FirstOrDefault(k => k.Key == "ListPrice").Value;

            this.Category = p.CategoryProducts.FirstOrDefault().Category;

            return this;
        }
    }
}