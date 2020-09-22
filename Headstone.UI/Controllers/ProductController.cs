using Headstone.Framework.Models;
using Headstone.MetaData.SDK.Net._452;
using Headstone.Models;
using Headstone.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class ProductController : BaseController
    {
        private MetaDataServiceClient metadataServiceClient = new MetaDataServiceClient();
        private Headstone.Service.CommentService commentService = new Service.CommentService();

        [Route("urunler/{catId}")]
        public ActionResult List(int catId)
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ürün Listesi" // degisecek
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            var productList = metadataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            {
                CategoryIds = new List<int> { catId },
                Envelope = "full"
            }).Result;

            var categoryList = metadataServiceClient.GetCategories(new MetaData.API.Models.Queries.Live.CategoryQuery()
            {
                ProductIds = productList.Select(p => p.ProductId).ToList()
            }).Result;

            var model = new ProductListViewModel();

            model.Products = productList.Select(p => new ProductViewModel().From(p)).ToList();

            model.Categories = categoryList.Select(c => new CategoryViewModel().From(c)).ToList();

            return View(model);
        }

        [Route("urun/{id}")]
        public ActionResult Details(int id)
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ürün Ismi" // degisecek
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            try
            {
                var product = metadataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
                {
                    ProductIds = new List<int> { id },
                    Envelope = "full"
                }).Result.FirstOrDefault();

                ProductViewModel model = new ProductViewModel().From(product);

                //get the comments 
                var commentResponse = commentService.GetComments(new Headstone.Models.Requests.CommentRequest()
                {
                    RelatedDataEntityTypes = new List<string> { "Product" },
                    RelatedDataEntityIds = new List<int> { product.ProductId }
                }).Result;

                if (commentResponse.Any())
                {
                    model.Comments = commentResponse.Select(c => new CommentViewModel()
                    {
                        Comment = c.Body,
                        Rating = (int)c.Rating,
                        Commenter = identityServiceClient.GetUsers(new Lidia.Identity.API.Models.Queries.UserQuery()
                        {
                            RelatedUserIds = new List<int> { c.UserId }
                        }).Result.FirstOrDefault()
                    }).ToList();
                }

                return View(model);

            }
            catch (Exception ex)
            {
                //Log

                return RedirectToAction("List");
            }
        }
    }
}