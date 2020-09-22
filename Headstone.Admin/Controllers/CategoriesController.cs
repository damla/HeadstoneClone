using Headstone.AI.Models.ViewModels;
using Headstone.Service.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Models;
using Headstone.MetaData.API.Models.Commands.Live;
using Headstone.MetaData.API.Models.Live;
using Headstone.MetaData.API.Models.Queries.Live;
using Headstone.MetaData.Common.Models;
using Headstone.MetaData.SDK.Net._452;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("categories")]
    public class CategoriesController : BaseController
    {
        private MetaDataServiceClient _metadataServiceClient = new MetaDataServiceClient();

        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Kategoriler"
                
            });
                        
            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        [HttpGet, Process(Name = "Categories.New")]
        public ActionResult New()
        {
            var model = new CategoryViewModel();

            model = this.CreateSelectLists(model);

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Kategoriler",
                Link = "/Categories"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Yeni"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View(model);
        }

        [HttpPost, Process(Name = "Categories.New")]
        [ValidateAntiForgeryToken]
        public ActionResult New(CategoryViewModel model)
        {
            // Add it to http context
            try
            {
                var command = new CategoryCommand()
                {
                    Name = model.Name,
                    Code = model.Code,
                    CRMCode = model.CRMCode,
                    Type = model.Type,
                    ShortDescription = model.ShortDescription,
                    LongDescription = model.LongDescription,
                    SortOrder = model.SortOrder,
                    Status = model.Status,
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId
                };

                var response = _metadataServiceClient.CreateCategory(command);

                model.CategoryId = response.CategoryId;

                if (response.Errors.Any())
                {
                    model.Errors = response.Errors;
                }
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, "There is an error while creating a category!", ex);

                model.Errors.Add(ex.Message);

                return RedirectToAction("Details", new { id = model.CategoryId });
            }

            return RedirectToAction("Details", new { id = model.CategoryId });
        }

        [HttpGet, Process(Name = "Categories.Details")]
        public ActionResult Details(int id)
        {
            // Create the model
            var model = new CategoryDetailsViewModel();
            try
            {
                // Create the query
                var query = new CategoryQuery
                {
                    CategoryIds = new List<int> { id },
                    Envelope = "full"
                };

                //Get the category
                var categoryResponse = _metadataServiceClient.GetCategories(query);

                //Check for errors 
                if (categoryResponse.Errors.Any())
                {
                    categoryResponse.Errors.ForEach(error => model.Errors.Add(error));
                }

                var result = categoryResponse.Result.FirstOrDefault();

                model.Category = new CategoryViewModel()
                {
                    CategoryId = result.CategoryId,
                    Name = result.Name,
                    LongDescription = result.LongDescription,
                    Type = result.Type,
                    ParentId = result.ParentId,
                    Code = result.Code,
                    CRMCode = result.CRMCode,
                    Status = result.Status,
                    SortOrder = result.SortOrder,
                    Products = result.Products,
                    Children = result.Children,
                    Parent = result.Parent,
                    Properties = result.Properties,
                    Tags = result.Tags,
                    ShortDescription = result.ShortDescription,
                    Pictures = result.Pictures
                };

                model.Category = this.CreateSelectLists(model.Category);

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Kategoriler",
                    Link = "/Categories"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = result.Name
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the category details! CategoryId:{id}", ex);
                model.Errors.Add(ex.Message);
                return View(model);
            }
        }

        [HttpPost, Process(Name = "Categories.Details")]
        public ActionResult Details(CategoryViewModel model)
        {
            try
            {
                // Get the exist category
                var category = _metadataServiceClient.GetCategories(new CategoryQuery()
                {
                    CategoryIds = new List<int> { model.CategoryId },
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId,
                    Envelope = "full"
                }).Result.FirstOrDefault();

                var command = new CategoryCommand()
                {
                    CategoryId = model.CategoryId,
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Code = model.Code,
                    CRMCode = model.CRMCode,
                    Type = model.Type,
                    ShortDescription = model.ShortDescription,
                    LongDescription = model.LongDescription,
                    SortOrder = model.SortOrder,
                    Status = model.Status,
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId
                };

                var response = _metadataServiceClient.UpdateCategory(command);

                if (response.Errors.Any())
                {
                    response.Errors.ForEach(error => model.Errors.Add(error));
                }

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Kategoriler",
                    Link = "/Categories"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = model.Name
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while updating the category details! CategoryId:{model.CategoryId}", ex);
                model.Errors.Add(ex.Message);
                return View("Details", model);
            }
            return RedirectToAction("Details", new { id = model.CategoryId });
        }

        #region [ Data functions ]

        [Process(Name = "Categories.GetCategories")]
        public JsonResult GetCategories([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var query = new CategoryQuery();

                var result = _metadataServiceClient.GetCategories(query);
                var response = result.Result.Select(r => new CategoryViewModel
                {
                    CategoryId = r.CategoryId,
                    Name = r.Name,
                    LongDescription = r.LongDescription,
                    Type = r.Type,
                    ParentId = r.ParentId,
                    ShortDescription = r.ShortDescription,
                }).ToList().OrderByDescending(i => i.CategoryId).ToDataSourceResult(req);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the category data!", ex);
                ModelState.AddModelError("Get Categories", ex.Message);
                var result = ModelState.ToDataSourceResult();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public List<Category> GetRootCategoriesFromCategories(Category category, List<Category> rootCategories)
        {
            if(category.Children.Count == 0)
            {
                return rootCategories;
            }
            try
            {
                // Add children to list
                foreach (var root in category.Children)
                {
                    rootCategories.Add(root);

                    foreach (var subroot in root.Children)
                    {
                        rootCategories.Add(subroot);

                        foreach (var subroot2 in subroot.Children)
                        {
                            rootCategories.Add(subroot2);

                            foreach (var subroot3 in subroot2.Children)
                            {
                                rootCategories.Add(subroot3);

                                foreach (var subroot4 in subroot3.Children)
                                {
                                    rootCategories.Add(subroot4);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var temasdap = "";
            }

            return rootCategories;
        }

        #endregion

        #region [ Private functions ]

        private CategoryViewModel CreateSelectLists(CategoryViewModel model)
        {
            try
            {
                var categoryTypes = GetAll<CategoryType>();
                var parentCategoriesResponse = _metadataServiceClient.GetCategories(new CategoryQuery());

                //Check for errors
                if (parentCategoriesResponse.Errors.Any())
                {
                    parentCategoriesResponse.Errors.ForEach(error => model.Errors.Add(error));
                }
                var parentCategories = parentCategoriesResponse.Result;
                var categoryStatus = new Dictionary<Headstone.Framework.Models.EntityStatus, string>
                {
                    { Headstone.Framework.Models.EntityStatus.Active, "Aktif" },
                    { Headstone.Framework.Models.EntityStatus.Passive, "Pasif" }
                };
                var parentSelectList = parentCategories.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.CategoryId.ToString()
                }).ToList();

                parentSelectList.Add(new SelectListItem()
                {
                    Text = "Hiçbiri",
                    Value = "",
                    Selected = true
                });

                model.ParentList = parentSelectList;
                model.Types = new SelectList(categoryTypes.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");
                model.Statuses = new SelectList(categoryStatus.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");
            }
            catch (Exception ex)
            {
                model.Errors.Add(ex.Message);
                Log(LogMode.Error, "There is an error in create category page!", ex);
            }
            return model;

        }

        #endregion
    }
}