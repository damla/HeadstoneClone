using Headstone.AI.Models.ViewModels;
using Headstone.Models.Requests;
using Headstone.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Logging;
using Lidia.Identity.SDK.Net.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using Lidia.Identity.API.Models.Queries;
using System.Collections.Generic;
using Lidia.Identity.API.Models.Commands;
using Headstone.Common;
using Headstone.MetaData.SDK.Net._452;
using Headstone.MetaData.API.Models.Queries.Live;
using Headstone.Framework.Models;
using Headstone.Service.Attributes;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("contents")]
    public class ContentController : BaseController
    {
        private CommentService _commentService;
        private IdentityServiceClient _identityService;
        private MetaDataServiceClient _metadataServiceClient;

        public ContentController()
        {
            _commentService = new CommentService();
            _identityService = new IdentityServiceClient();
            _metadataServiceClient = new MetaDataServiceClient();
        }

        // GET: Content
        [Route("customerreviews")]
        public ActionResult CustomerReviews()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Yorumlar Ve Puanlar"                
            });
                        
            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        [Route("customerreviews/{id}"), Process(Name = "Content.CustomerReviewDetails")]
        public ActionResult CustomerReviewDetails(int id)
        {
            // Create the model
            var model = new CustomerReviewDetailsViewModel();
            try
            {
                // Create the comment query
                var request = new CommentRequest()
                {
                    CommentIds = new List<int> { id }
                };

                // Get the comment
                var response = _commentService.GetComments(request).Result.FirstOrDefault();

                // Set the comment data 
                model.CustomerReview = new CustomerReviewViewModel
                {
                    Body = response.Body,
                    CommentId = response.CommentId,
                    Created = response.Created.ToString("dd-MM-yyyy HH:mm"),
                    Status = response.Status,
                    ParentId = response.ParentId,
                    Rating = response.Rating,
                    RelatedDataEntityId = response.RelatedDataEntityId,
                    RelatedDataEntityType = response.RelatedDataEntityType,
                    Type = response.Type,
                    Updated = response.Updated,
                    UserId = response.UserId
                };

                #region [ User ]

                // Create the user query
                var userQuery = new UserQuery()
                {
                    RelatedUserIds = new List<int> { response.UserId }
                };

                // Get the user
                var userResponse = _identityService.GetUsers(userQuery).Result.FirstOrDefault();

                // Set the source username
                model.SourceUserName = userResponse.Firstname + " " + userResponse.Lastname;

                #endregion

                #region [ Related Entity ]

                switch (response.RelatedDataEntityType)
                {
                    case "Product":

                        // Create the product query
                        var productQuery = new ProductQuery()
                        {
                            ProductIds = new List<int> { response.RelatedDataEntityId },
                            Envelope = "full"
                        };

                        // Get the product
                        var product = _metadataServiceClient.GetProducts(productQuery).Result.FirstOrDefault();

                        // Set the related data
                        model.Description = product.ShortDescription;
                        model.SourceId = product.ProductId;
                        model.SourceTitle = product.Name;
                        model.Created = product.Created.ToString("yyyy-MM-dd HH:mm");
                        model.Images = product.Pictures.Select(p=>p.ImageUrl).ToList();
                        break;

                    case "Trademark":

                        // Create the trademark query
                        var trademarkQuery = new TrademarkQuery()
                        {
                            TrademarkIds = new List<int> { response.RelatedDataEntityId },
                            Envelope = "full"
                        };

                        // Get the trademark
                        var trademark = _metadataServiceClient.GetTrademarks(trademarkQuery).Result.FirstOrDefault();

                        // Set the related data
                        model.Description = trademark.ShortDescription;
                        model.SourceId = trademark.TrademarkId;
                        model.SourceTitle = trademark.Name;
                        model.Created = trademark.Created.ToString("yyyy-MM-dd HH:mm");
                        model.Images = trademark.Pictures.Select(p => p.ImageUrl).ToList();
                        break;

                    case "Category":

                        // Create the category query
                        var categoryQuery = new CategoryQuery()
                        {
                            CategoryIds = new List<int> { response.RelatedDataEntityId },
                            Envelope = "full"
                        };

                        // Get the category
                        var category = _metadataServiceClient.GetCategories(categoryQuery).Result.FirstOrDefault();

                        // Set the related data
                        model.Description = category.ShortDescription;
                        model.SourceId = category.CategoryId;
                        model.SourceTitle = category.Name;
                        model.Created = category.Created.ToString("yyyy-MM-dd HH:mm");
                        break;
                }

                #endregion

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Yorumlar Ve Puanlar",
                    Link = "/contents/customerreviews"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = response.CommentId.ToString()
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the customer review details! CustomerReviewId:{id}", ex);
            }
            return View(model);
        }

        [HttpPost]
        [Route("customerreviews/{id}/edit"), Process(Name = "Content.CustomerReviewDetails")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        #region [ Data functions ]

        [Process(Name = "Content.GetComments")]
        public JsonResult GetComments([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                //Get the resellers
                var users = _identityService.GetUsers(new UserQuery()
                {

                }).Result.ToList();

                var request = new CommentRequest();
                var response = _commentService.GetComments(request);
                var result = response.Result.Select(c => new CustomerReviewViewModel()
                {
                    CommentId = c.CommentId,
                    ParentId = c.ParentId,
                    RelatedUserName = users.Where(x=>x.Id==c.UserId).FirstOrDefault().Firstname + users.Where(x => x.Id == c.UserId).FirstOrDefault().Lastname,
                    UserId = c.UserId,
                    RelatedDataEntityId = c.RelatedDataEntityId,
                    RelatedDataEntityType = c.RelatedDataEntityType,
                    Type = c.Type,
                    Rating = c.Rating,
                    Body = c.Body,
                    Created = c.Created.ToString("dd-MM-yyyy HH:mm"),
                    Updated = c.Updated,
                    Status = c.Status
                }).ToList().OrderByDescending(x => x.CommentId).ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log(LogMode.Error,$"There is an error while getting the comments!", e);

                return Json(e);
            }
        }

        #endregion
    }
}