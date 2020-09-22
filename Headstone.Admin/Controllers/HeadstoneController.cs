using Headstone.AI.Models.DataModels;
using Headstone.AI.Models.ViewModels;
using Headstone.Models;
using Headstone.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Logging;
using Headstone.MetaData.API.Models.Queries.Live;
using Headstone.MetaData.SDK.Net._452;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.MetaData.API.Models.Commands.Live;
using Headstone.Models.Events.Comment;
using Headstone.Models.Requests;
using Headstone.Service.Base;
using Headstone.Service.Interfaces;
using Lidia.Identity.API.Models.Queries;
using Lidia.Identity.SDK.Net.Identity;
using Lidia.Identity.API.Models.Commands;
using Lidia.Identity.API.Models;
using Headstone.MetaData.API.Models.Live;
using System.IO;
using System.Web.Routing;
using Headstone.AI.Models.Responses;
using Newtonsoft.Json;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("lm")]
    public class HeadtsoneController : BaseController
    {
        private CommentService commentService = new CommentService();
        private IdentityServiceClient identityService = new IdentityServiceClient();
        private GeoLocationService geoLocationService = new GeoLocationService();
        private MetaDataServiceClient metadataServiceClient = new MetaDataServiceClient();


        // File directories
        private string fileTempDirectoryPath = string.Empty;
        private DirectoryInfo fileTempDiretory = null;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            // Set the image temp directory
            fileTempDirectoryPath = Server.MapPath("~/Temp/Files");
            fileTempDiretory = new DirectoryInfo(fileTempDirectoryPath);
        }

        #region [ Categories ]

        [Route("getproductbycategory")]
        public JsonResult GetProductsByCategory([DataSourceRequest] DataSourceRequest req, int categoryId)
        {
            try
            {
                // Create the category query
                var categoryQuery = new CategoryQuery()
                {
                    CategoryIds = new List<int> { categoryId },
                    Envelope = "full"
                };

                var category = metadataServiceClient.GetCategories(categoryQuery).Result.FirstOrDefault();

                // Get all root categories
                var rootcategories = GetRootCategoriesFromCategories(category, new List<Category>());

                // Get the categoy ids 
                var categoryids = rootcategories.Select(s => s.CategoryId).ToList();

                // Add selected category id to list
                categoryids.Add(categoryId);

                if (categoryids.Count != 0)
                {
                    // Create the product query
                    var productQuery = new ProductQuery()
                    {
                        CategoryIds = categoryids
                    };

                    // Get the products
                    var result = metadataServiceClient.GetProducts(productQuery);

                    // Set the data to model
                    var response = result.Result.Select(r => new ProductViewModel
                    {
                        ProductId = r.ProductId,
                        Name = r.Name,
                        ShortDescription = r.ShortDescription
                    }).ToList().OrderByDescending(i => i.ProductId).ToDataSourceResult(req);

                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ProductViewModel(), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public JsonResult UpdateCategoryStatus(int cid, short status)
        {
            var categoryUpdateStatus = metadataServiceClient.UpdateCategoryStatus(new CategoryCommand()
            {
                CategoryId = cid,
                Status = (Headstone.MetaData.API.Models.EntityStatus)status,
                UserToken = CurrentUser.UserToken,
                Environment = "Dev",
                SessionId = CurrentUser.SessionId
            });

            if (categoryUpdateStatus.Type !=Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
            {
                return Json(new headstoneControllerResponse
                {
                    ReturnCode = -300
                });
            }

            return Json(new headstoneControllerResponse
            {
                ReturnCode = 200
            });
        }

        #region [ Data functions ]

        public List<Category> GetRootCategoriesFromCategories(Category category, List<Category> rootCategories)
        {
            if (category.Children.Count == 0)
            {
                return rootCategories;
            }
            try
            {
                // Add children to list ** 5 STEP **
                foreach (var root in category.Children)
                {
                    try
                    {
                        rootCategories.Add(root);

                        foreach (var subroot in root.Children)
                        {
                            try
                            {
                                rootCategories.Add(subroot);

                                foreach (var subroot2 in subroot.Children)
                                {
                                    try
                                    {
                                        rootCategories.Add(subroot2);

                                        foreach (var subroot3 in subroot2.Children)
                                        {
                                            try
                                            {
                                                rootCategories.Add(subroot3);

                                                foreach (var subroot4 in subroot3.Children)
                                                {
                                                    try
                                                    {
                                                        rootCategories.Add(subroot4);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while getting categories children step5 CategoryId: {subroot3.CategoryId}", ex);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while getting root categories step4 CategoryId: {subroot2.CategoryId}", ex);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while getting root categories step3 CategoryId: {subroot.CategoryId}", ex);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while getting root categories step2 CategoryId: {root.CategoryId}", ex);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while getting root categories step1 CategoryId: {category.CategoryId}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, "There is an error while getting all root categories", ex);

                return rootCategories;
            }

            return rootCategories;
        }

        #endregion

        #endregion

        #region [ Trademark ] 

        public JsonResult GetProductsByTrademark([DataSourceRequest] DataSourceRequest req, int trademarkId)
        {
            try
            {
                var query = new TrademarkQuery
                {
                    TrademarkIds = new List<int> { trademarkId },
                    Envelope = "full"
                };
                var trademarkResult = metadataServiceClient.GetTrademarks(query).Result.FirstOrDefault();

                var productIds = trademarkResult.Products.Select(p => p.ProductId).ToList();

                if (productIds.Count <= 0)
                {
                    return Json(new ProductViewModel(), JsonRequestBehavior.AllowGet);
                }

                var productQuery = new ProductQuery
                {
                    ProductIds = productIds
                };

                var result = metadataServiceClient.GetProducts(productQuery);
                var response = result.Result.Select(r => new ProductViewModel
                {
                    ProductId = r.ProductId,
                    Name = r.Name,
                    ShortDescription = r.ShortDescription
                }).ToList().OrderByDescending(i => i.ProductId).ToDataSourceResult(req);

                return Json(response, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        public JsonResult UpdateTrademarkStatus(int tid, short status)
        {
            var trademarkUpdateStatus = metadataServiceClient.UpdateTrademarkStatus(new TrademarkCommand()
            {
                TrademarkId = tid,
                Status = (Headstone.MetaData.API.Models.EntityStatus)status
            });

            if (trademarkUpdateStatus.Type !=Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
            {
                return Json(new headstoneControllerResponse
                {
                    ReturnCode = -300
                });
            }

            return Json(new headstoneControllerResponse
            {
                ReturnCode = 200
            });
        }

        #endregion

        #region [ Comment ]

        public JsonResult UpdateCommentStatus(int cid, short status)
        {
            var commentUpdateResponse = commentService.UpdateCommentStatus(new CommentUpdated()
            {
                CommentId = cid,
                Status = (Headstone.Framework.Models.EntityStatus)status
            });

            if (commentUpdateResponse.Type != Common.ServiceResponseTypes.Success)
            {
                return Json(new headstoneControllerResponse
                {
                    ReturnCode = -300
                });
            }

            return Json(new headstoneControllerResponse
            {
                ReturnCode = 200
            });
        }

        public JsonResult DeleteComment(int cid)
        {
            var commentDeleteResponse = commentService.DeleteComment(new CommentDeleted()
            {
                CommentId = cid,
                Environment = this.Environment,
                ApplicationIP = HttpContext.Request.UserHostAddress,
                UserToken = this.UserToken,
                SessionId = SessionId
            });

            if (commentDeleteResponse.Type != Common.ServiceResponseTypes.Success)
            {
                return Json(new headstoneControllerResponse
                {
                    ReturnCode = -300
                });
            }

            // Get the comment 
            var comment = commentService.GetComments(new CommentRequest()
            {
                CommentIds = new List<int> { cid }
            }).Result.FirstOrDefault();

            //Not implemented

            return Json(new headstoneControllerResponse
            {
                ReturnCode = 200
            });
        }

        #endregion

        #region [ User ]
        public JsonResult UpdateUserStatus(int uid, short status)
        {
            var userUpdateResponse = identityService.UpdateUser(new UserCommand()
            {
                Status = (Lidia.Identity.Common.Models.EntityStatus)status,
                Environment = this.Environment,
                UserToken = this.UserToken,
                SessionId = SessionId,
                UserId = CurrentUser.Id,
                RelatedUserId = uid
            });

            if (userUpdateResponse.Type != Lidia.Identity.Common.Models.ServiceResponseTypes.Success)
            {
                return Json(new headstoneControllerResponse
                {
                    ReturnCode = -300
                });
            }

            // Get the user
            var user = identityService.GetUsers(new UserQuery()
            {
                Environment = this.Environment,
                UserToken = this.UserToken,
                SessionId = SessionId,
                UserId = CurrentUser.Id,
                Envelope = "full",
                RelatedUserIds = new List<int> { uid }
            }).Result.FirstOrDefault();

            // Maybe send mail 
            if (user.Status != Lidia.Identity.Common.Models.EntityStatus.Freezed || user.Status != Lidia.Identity.Common.Models.EntityStatus.Deleted)
            {
                return Json(new headstoneControllerResponse
                {
                    ReturnCode = 200
                });
            }

            return Json(new headstoneControllerResponse
            {
                ReturnCode = 200
            });
        }

        #endregion

        #region [ Service ]

        public JsonResult GetDistrictByCity(string path)
        {
            try
            {
                // Get the geo locations
                var response = geoLocationService.GetGeoLocations(new GeoLocationRequest()
                {
                    GeoLocationParents = new List<string> { path }
                }).Result.ToList();

                return Json(new headstoneControllerResponse<GeoLocation>()
                {
                    Result = response,
                    ReturnCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while getting district by city!", ex);

                return Json(new headstoneControllerResponse<Category>()
                {
                    ReturnCode = -300
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [ Metadata ]

        [Route("metadata/deleteproductpicture")]
        public JsonResult DeleteProductPicture(int productId, int pictureId)
        {
            try
            {
                // Get the offer 
                var product = metadataServiceClient.GetProducts(new ProductQuery()
                {
                    ProductIds = new List<int> { productId },
                    Envelope = "full"
                }).Result.FirstOrDefault();

                if (User.IsInRole("Administrator"))
                {
                    // Get the removal picture
                    var picture = product.Pictures.Where(p => p.PictureId == pictureId).FirstOrDefault();

                    // Delete the picture from metadata
                    var response = metadataServiceClient.DeleteProductPicture(new ProductPictureCommand()
                    {
                        Environment = this.Environment,
                        UserToken = this.UserToken,
                        SessionId = this.SessionId,
                        PictureId = pictureId,
                        ApplicationIP = HttpContext.Request.UserHostAddress,
                        ProductId = productId
                    });

                    // Check the response 
                    if (response.Type !=Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                    {
                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while deleting the product picture! Error:{response.Message}", null);

                        return Json(new headstoneControllerResponse()
                        {
                            ReturnCode = -300
                        }, JsonRequestBehavior.AllowGet);
                    }

                    // Get the product pictures
                    var newProductpictures = metadataServiceClient.GetProductPictures(new ProductPictureQuery()
                    {
                        ProductIds = new List<int> { Convert.ToInt32(productId) },
                        Envelope = "full"
                    }).Result.ToList();

                    var picturemodel = newProductpictures.Select(s => new ProductPicture()
                    {
                        Alt = s.Alt,
                        PictureId = s.PictureId,
                        ProductId = s.ProductId,
                        ImageUrl = s.ImageUrl
                    }).ToList();

                    // Create the model
                    var model = new ProductPictureListViewModel()
                    {
                        PictureKey = CreateThreadId(5),
                        ProductId = Convert.ToInt32(productId),
                        Pictures = picturemodel
                    };

                    // Convert partial to string
                    var partial = ViewToString("Products", "~/Views/Products/Pictures.cshtml", model);

                    return Json(new headstonePartialResponse()
                    {
                        ReturnCode = 200,
                        Html = partial
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    Log(Headstone.Framework.Models.LogMode.Error, $"Authorize failure", null);

                    return Json(new headstoneControllerResponse()
                    {
                        ReturnCode = -300
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while deleting the offer picture!", ex);

                return Json(new headstoneControllerResponse()
                {
                    ReturnCode = -300
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateProductStatus(int pid, short status)
        {
            // Get the exist product
            var product = metadataServiceClient.GetProducts(new ProductQuery()
            {
                ProductIds = new List<int> { pid },
                Environment = this.Environment,
                UserToken = this.UserToken,
                SessionId = SessionId,
                UserId = CurrentUser.Id
            }).Result.FirstOrDefault();

            // Update product status
            var productUpdateStatus = metadataServiceClient.UpdateProduct(new ProductCommand()
            {
                Environment = this.Environment,
                UserToken = this.UserToken,
                SessionId = SessionId,
                UserId = CurrentUser.Id,
                ProductId = pid,
                Status = (Headstone.MetaData.API.Models.EntityStatus)status,
                Code = product.Code,
                ERPCode = product.ERPCode,
                LongDescription = product.LongDescription,
                ShortDescription = product.ShortDescription,
                Name = product.Name,
                Type = product.Type,
                Unit = product.Unit,
                ApplicationId = product.ApplicationId,
                TenantId = product.TenantId.Value,
                CRMCode = product.CRMCode,
            });

            // Check the response
            if (productUpdateStatus.Type !=Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
            {
                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while updating product status! ProductId:{pid} Error:{String.Join(",", productUpdateStatus.Errors)}", null);

                return Json(new headstoneControllerResponse
                {
                    ReturnCode = -300
                });
            }

            return Json(new headstoneControllerResponse
            {
                ReturnCode = 200
            });
        }

        #endregion

    }
}


