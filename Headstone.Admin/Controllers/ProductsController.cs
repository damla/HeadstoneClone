using Headstone.AI.Models.DataModels;
using Headstone.AI.Models.ViewModels;
using Headstone.Service;
using Headstone.Service.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Logging;
using Headstone.Framework.Models;
using Headstone.MetaData.API.Models.Commands.Live;
using Headstone.MetaData.API.Models.Live;
using Headstone.MetaData.API.Models.Queries.Live;
using Headstone.MetaData.SDK.Net._452;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("products")]
    public class ProductsController : BaseController
    {
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

        [HttpGet]
        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ürünler"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        [Route("new")]
        public ActionResult New()
        {
            var model = new CreateProductViewModel()
            {
                CurrentUser = CurrentUser,
                TempPictureKey = CreateThreadId(5)
            };

            try
            {
                // Get the categories
                var categoryResponse = metadataServiceClient.GetCategories(new CategoryQuery()
                {
                    Envelope = "full"
                });

                if (CurrentUser.Status == Lidia.Identity.Common.Models.EntityStatus.Draft)
                {
                    model.Errors.Add("Üyeliğiniz henüz onaylanmamıştır. Üyeliğiniz onaylandığında size bilgi verilecektir.");

                    return View(model);
                }

                // Get the trademarks
                var trademarks = metadataServiceClient.GetTrademarks(new TrademarkQuery());

                // Get the listing categories
                var categories = categoryResponse.Result.ToList();

                var parentList = new List<CategoryViewModel>();

                // Get parent list
                foreach (var category in categories)
                {
                    // Get the path
                    var idPath = GetIDPath(category);
                    var namePath = GetNamePath(category);

                    parentList.Add(new CategoryViewModel()
                    {
                        CategoryId = category.CategoryId,
                        ParentId = category.ParentId,
                        Parent = category.Parent,
                        IdPath = idPath,
                        NamePath = namePath,
                        Name = category.Name,
                        IsLeaf = category.Children.Count == 0
                    });
                }

                // Set data to model
                model.Categories = parentList;
                model.Trademarks = trademarks.Result.ToList();

            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, "There is an error while creating the product", ex);
            }

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ürünler",
                Link = "/products"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Yeni"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View(model);
        }

        [HttpPost, Route("new")]
        public ActionResult New(CreateProductViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Create the command
                    var command = new ProductCommand()
                    {
                        Name = model.Name,
                        Status = Headstone.MetaData.API.Models.EntityStatus.Active,
                        Type = Headstone.MetaData.API.Models.ProductType.Physical,
                        Code = model.Code,
                        Environment = this.Environment,
                        UserToken = this.UserToken,
                        SessionId = this.SessionId,
                        ThreadId = ThreadId,
                        Properties = new List<ProductProperty>()
                        {
                        new ProductProperty()
                        {
                            Created = DateTime.UtcNow,
                            Key = "Price",
                            Value = model.Price.ToString()
                        },
                         new ProductProperty()
                        {
                            Created = DateTime.UtcNow,
                            Key = "ListPrice",
                            Value = model.ListPrice.ToString()
                        }
                        },
                    };

                    // Add the product
                    var productResponse = metadataServiceClient.CreateProduct(command);

                    // Check the response 
                    if (productResponse.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                    {
                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while creating the product! Error:{productResponse.Message}", null);

                        return View(model);
                    }

                    #region [ Categories ]

                    foreach (var category in model.CategoryIds)
                    {
                        // Add categories 
                        var categoryResponse = metadataServiceClient.CreateCategoryProduct(new CategoryProductCommand()
                        {
                            Environment = this.Environment,
                            UserToken = this.UserToken,
                            SessionId = this.SessionId,
                            ThreadId = ThreadId,
                            ProductId = productResponse.ProductId,
                            CategoryId = Convert.ToInt32(category),
                            Status = Headstone.MetaData.API.Models.EntityStatus.Active
                        });

                        // Check the category response 
                        if (categoryResponse.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                        {
                            Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while creating the category product! Error:{categoryResponse.Message}", null);

                            return View(model);
                        }
                    }

                    #endregion

                    #region [ Trademark ]

                    // Add trademark
                    var trademarkResponse = metadataServiceClient.CreateTrademarkProduct(new TrademarkProductCommand()
                    {
                        Environment = this.Environment,
                        UserToken = this.UserToken,
                        SessionId = this.SessionId,
                        ThreadId = ThreadId,
                        ProductId = productResponse.ProductId,
                        TrademarkId = Convert.ToInt32(model.Trademark),
                        Status = Headstone.MetaData.API.Models.EntityStatus.Active,
                        SortOrder = 1
                    });

                    // Check the category response 
                    if (trademarkResponse.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                    {
                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while creating the trademark product! Error:{trademarkResponse.Message}", null);

                        return View(model);
                    }


                    return RedirectToAction("Details", new { id = productResponse.ProductId });

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, "There is an error while creating the products", ex);

                model.Errors.Add("Ürün eklerken hata oluştu");
            }
            return RedirectToAction("Products", "My");
        }

        [HttpGet, Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            // Create the model
            var model = new CreateProductViewModel()
            {
                CurrentUser = CurrentUser,
                TempPictureKey = CreateThreadId(5),
                IsEdit = true
            };

            try
            {
                // Load the exist product
                var product = metadataServiceClient.GetProducts(new ProductQuery()
                {
                    Envelope = "full",
                    ProductIds = new List<int> { id }
                }).Result.FirstOrDefault();

                model.Name = product.Name;
                model.Pictures = product.Pictures;
                model.SelectedTrademarkId = product.TrademarkProducts.FirstOrDefault().Trademark.TrademarkId;
                model.EANCode = product.Properties.Where(x => x.Key == "EANCode").FirstOrDefault()?.Value;
                model.Code = product.Code;
                model.ProductId = id;

                // Get the produc categories
                var productcategories = product.CategoryProducts.Select(c => c.CategoryId).ToList();
                model.SelectedCategoryIds = productcategories;

                // Get the categories
                var categoryResponse = metadataServiceClient.GetCategories(new CategoryQuery()
                {
                    Envelope = "full"
                });

                if (CurrentUser.Status == Lidia.Identity.Common.Models.EntityStatus.Draft)
                {
                    model.Errors.Add("Üyeliğiniz henüz onaylanmamıştır. Üyeliğiniz onaylandığında size bilgi verilecektir.");

                    return View(model);
                }

                // Get the trademarks
                var trademarks = metadataServiceClient.GetTrademarks(new TrademarkQuery());

                // Get the listing categories
                var categories = categoryResponse.Result.ToList();

                var parentList = new List<CategoryViewModel>();

                // Get parent list
                foreach (var category in categories)
                {
                    // Get the path
                    var idPath = GetIDPath(category);
                    var namePath = GetNamePath(category);

                    parentList.Add(new CategoryViewModel()
                    {
                        CategoryId = category.CategoryId,
                        ParentId = category.ParentId,
                        Parent = category.Parent,
                        IdPath = idPath,
                        NamePath = namePath,
                        Name = category.Name,
                        IsLeaf = category.Children.Count == 0
                    });
                }

                // Set data to model
                model.Categories = parentList;
                model.Trademarks = trademarks.Result.ToList();

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Ürünler",
                    Link = "/urunler"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = model.Name,
                    Link = "/urun/" + model.ProductId
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Düzenle"
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, "There is an error while creating the product", ex);
            }

            return View("New", model);
        }

        [HttpPost, Route("edit/{id}"), ValidateAntiForgeryToken]
        public ActionResult Edit(CreateProductViewModel model)
        {
            try
            {
                #region [ Load model data ]

                var existmodel = new CreateProductViewModel()
                {
                    CurrentUser = CurrentUser,
                    TempPictureKey = CreateThreadId(5),
                    IsEdit = true
                };

                // Load the exist product
                var product = metadataServiceClient.GetProducts(new ProductQuery()
                {
                    Envelope = "full",
                    ProductIds = new List<int> { model.ProductId }
                }).Result.FirstOrDefault();

                existmodel.Name = model.Name;
                existmodel.Pictures = product.Pictures;
                existmodel.SelectedTrademarkId = Convert.ToInt32(model.Trademark);
                existmodel.EANCode = model.EANCode;
                existmodel.Code = model.Code;
                existmodel.ProductId = model.ProductId;
                //existmodel.TaxRate = model.TaxRate,

                // Get the produc categories
                var productcategories = product.CategoryProducts.Select(c => c.CategoryId).ToList();
                model.SelectedCategoryIds = productcategories;

                // Get the categories
                var categoryResponse = metadataServiceClient.GetCategories(new CategoryQuery()
                {
                    Envelope = "full"
                });

                // Get the trademarks
                var trademarks = metadataServiceClient.GetTrademarks(new TrademarkQuery());

                // Get the listing categories
                var categories = categoryResponse.Result.ToList();

                var parentList = new List<CategoryViewModel>();

                // Get parent list
                foreach (var category in categories)
                {
                    // Get the path
                    var idPath = GetIDPath(category);
                    var namePath = GetNamePath(category);

                    parentList.Add(new CategoryViewModel()
                    {
                        CategoryId = category.CategoryId,
                        ParentId = category.ParentId,
                        Parent = category.Parent,
                        IdPath = idPath,
                        NamePath = namePath,
                        Name = category.Name,
                        IsLeaf = category.Children.Count == 0
                    });
                }

                // Set data to model
                existmodel.Categories = parentList;
                existmodel.Trademarks = trademarks.Result.ToList();

                #endregion

                if (ModelState.IsValid)
                {
                    // Get exist product
                    var existProduct = metadataServiceClient.GetProducts(new ProductQuery()
                    {
                        ProductIds = new List<int> { model.ProductId }
                    }).Result.FirstOrDefault();

                    var productCommand = new ProductCommand()
                    {
                        Environment = this.Environment,
                        UserToken = this.UserToken,
                        ProductId = model.ProductId,
                        SessionId = this.SessionId,
                        Status = Headstone.MetaData.API.Models.EntityStatus.Active,
                        ThreadId = ThreadId,
                        Name = model.Name,
                        Code = model.Code,
                        //TaxRate = model.TaxRate,
                    };

                    #region [ Product Properties ]

                    #region [ EAN Code ]

                    if (!String.IsNullOrEmpty(model.EANCode))
                    {
                        // Check the any exist property
                        var anyExistEAN = existProduct.Properties.Where(s => s.Key == "EANCode").Any();

                        if (anyExistEAN)
                        {
                            // Get the exist ean
                            var existEAN = existProduct.Properties.Where(s => s.Key == "EANCode").FirstOrDefault();

                            // Check the different EAN 
                            var isEqual = existProduct.Properties.Where(s => s.Key == "EANCode" && s.Value == model.EANCode).Any();

                            if (!isEqual)
                            {
                                // Delete the exist property
                                var propertyResponse = metadataServiceClient.DeleteProductProperty(new ProductPropertyCommand()
                                {
                                    PropertyId = existEAN.PropertyId,
                                    Environment = this.Environment,
                                    UserToken = this.UserToken,
                                    ProductId = model.ProductId,
                                    SessionId = this.SessionId,
                                    ThreadId = ThreadId,
                                    UserId = CurrentUser.Id
                                });

                                // Check the property response 
                                if (propertyResponse.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                                {
                                    Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while deleting the product property! Error:{propertyResponse.Message}", null);

                                    existmodel.Errors.Add("Ürün güncellenirken hata oluştu !");

                                    return View("Create", existmodel);
                                }

                                // Create the product property
                                var propertycommand = metadataServiceClient.CreateProductProperty(new ProductPropertyCommand()
                                {
                                    Environment = this.Environment,
                                    UserToken = this.UserToken,
                                    SessionId = this.SessionId,
                                    ThreadId = ThreadId,
                                    UserId = CurrentUser.Id,
                                    ProductId = model.ProductId,
                                    Key = "EANCode",
                                    Value = model.EANCode
                                });

                                // Check the property command
                                if (propertycommand.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                                {
                                    Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while creating the product property! Error:{propertycommand.Message}", null);

                                    existmodel.Errors.Add("Ürün güncellenirken hata oluştu !");

                                    return View("Create", existmodel);
                                }
                            }
                        }
                        else
                        {
                            // Create the product property
                            var propertycommand = metadataServiceClient.CreateProductProperty(new ProductPropertyCommand()
                            {
                                Environment = this.Environment,
                                UserToken = this.UserToken,
                                SessionId = this.SessionId,
                                ThreadId = ThreadId,
                                UserId = CurrentUser.Id,
                                ProductId = model.ProductId,
                                Key = "EANCode",
                                Value = model.EANCode
                            });

                            // Check the property command
                            if (propertycommand.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                            {
                                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while creating the product property! Error:{propertycommand.Message}", null);

                                existmodel.Errors.Add("Ürün güncellenirken hata oluştu !");

                                return View("Create", existmodel);
                            }
                        }
                    }
                    else
                    {
                        // Check the any exist property
                        var anyExistEAN = existProduct.Properties.Where(s => s.Key == "EANCode").Any();

                        if (anyExistEAN)
                        {
                            // Get the exist ean
                            var existEAN = existProduct.Properties.Where(s => s.Key == "EANCode").FirstOrDefault();

                            // Delete the exist property
                            var propertyResponse = metadataServiceClient.DeleteProductProperty(new ProductPropertyCommand()
                            {
                                PropertyId = existEAN.PropertyId,
                                Environment = this.Environment,
                                UserToken = this.UserToken,
                                ProductId = model.ProductId,
                                SessionId = this.SessionId,
                                ThreadId = ThreadId,
                                UserId = CurrentUser.Id
                            });

                            // Check the property response 
                            if (propertyResponse.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                            {
                                Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while deleting the product property! Error:{propertyResponse.Message}", null);

                                existmodel.Errors.Add("Ürün güncellenirken hata oluştu !");

                                return View("Create", existmodel);
                            }
                        }
                    }

                    #endregion

                    #endregion

                    #region [ Categories ]

                    if (model.CategoryIds.Count() != 0)
                    {

                        var categoryList = new List<CategoryProduct>();

                        // Add categories to object
                        foreach (var id in model.CategoryIds)
                        {
                            var categoryProductCommand = new CategoryProduct()
                            {
                                ProductId = model.ProductId,
                                CategoryId = Convert.ToInt32(id),
                                Status = Headstone.MetaData.API.Models.EntityStatus.Active
                            };
                            categoryList.Add(categoryProductCommand);
                        }

                        // Add categories to command
                        productCommand.Categories = categoryList;
                    }

                    #endregion

                    // Update the product 
                    var response = metadataServiceClient.UpdateProduct(productCommand);

                    // Check the response 
                    if (response.Type != Headstone.MetaData.Common.Models.ServiceResponseTypes.Success)
                    {
                        Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while updating the product! Error:{response.Message}", null);

                        existmodel.Errors.Add("Ürün güncellenirken hata oluştu !");

                        return View("Create", existmodel);
                    }
                }
                else
                {
                    return View("Create", existmodel);
                }
            }
            catch (Exception ex)
            {
                Log(Headstone.Framework.Models.LogMode.Error, "There is an error while creating the products(POST)", ex);
            }

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ürünler",
                Link = "/products"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = model.Name,
                Link = "/products/" + model.ProductId
            });


            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Düzenle"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return RedirectToAction("Details", new { id = model.ProductId });
        }

        [Process(Name = "Products.Details")]
        public ActionResult Details(int id)
        {
            var model = new ProductViewModel();

            try
            {
                #region [ Category ] 

                var categoryQuery = new CategoryQuery
                {
                    ProductIds = new List<int> { id },
                    Envelope = "full"
                };

                // Get the category
                var categoryResult = metadataServiceClient.GetCategories(categoryQuery).Result;

                //Set category
                model.Categories = categoryResult;

                #endregion

                #region [ Trademark ] 

                var trademarkQuery = new TrademarkQuery
                {
                    ProductIds = new List<int> { id },
                    Envelope = "full"
                };

                // Get the trademark
                var trademarkResult = metadataServiceClient.GetTrademarks(trademarkQuery).Result;
                //Set trademark
                model.Trademarks = trademarkResult;

                #endregion

                #region [ Product ] 

                // Create the query
                var query = new ProductQuery
                {
                    ProductIds = new List<int> { id },
                    Envelope = "full"
                };

                var result = metadataServiceClient.GetProducts(query).Result.FirstOrDefault();

                // Get the products
                model.Product = result;
                model.Categories = result.CategoryProducts.Select(a => a.Category).ToList();
                model.Trademarks = result.TrademarkProducts.Select(a => a.Trademark).ToList();

                #endregion

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Ürünler",
                    Link = "/products"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = result.Name
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the product details! ProductId:{id}", ex);

                return View();

            }
            return View(model);
        }

        #region [ Data functions ]

        [Process(Name = "Products.GetProducts")]
        public JsonResult GetProducts([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var productQuery = new ProductQuery();

                var result = metadataServiceClient.GetProducts(productQuery);
                var response = result.Result.Select(r => new ProductViewModel
                {
                    ProductId = r.ProductId,
                    Name = r.Name,
                    ShortDescription = r.ShortDescription,
                    Code = r.Code,
                    Status = (EntityStatus)r.Status,
                    //TaxRate = r.TaxRate,
                    ERPCode = r.ERPCode
                }).ToList().OrderByDescending(i => i.ProductId).ToDataSourceResult(req);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the product data!", ex);

                return Json(ex);
            }
        }

        #endregion
    }
}