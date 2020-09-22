using Headstone.AI.Models.ViewModels;
using Headstone.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Logging;
using Headstone.MetaData.API.Models.Queries.Live;
using Headstone.MetaData.SDK.Net._452;
using Headstone.MetaData.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.MetaData.API.Models.Commands.Live;
using Headstone.Framework.Models;
using Headstone.Service.Attributes;
using System.IO;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("trademarks")]
    public class TrademarksController : BaseController
    {
        private MetaDataServiceClient _metadataServiceClient = new MetaDataServiceClient();

        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Markalar"                
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        public ActionResult New()
        {
            var model = new TrademarkViewModel();
            model = this.CreateSelectLists(model);

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Markalar",
                Link = "/trademarks"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Yeni"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Process(Name = "Trademarks.New")]
        public ActionResult New(TrademarkViewModel model)
        {
            try
            {
                var command = new TrademarkCommand()
                {
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Code = model.Code,
                    ERPCode = model.ERPCode,
                    ShortDescription = model.ShortDescription,
                    LongDescription = model.LongDescription,
                    Status = model.Status,
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId
                };

                var response = _metadataServiceClient.CreateTrademark(command);

                model.TrademarkId = response.TrademarkId;

            }
            catch (Exception e)
            {
                Log(LogMode.Error, "There is an error while creating a trademark!", e);
                throw;
            }

            return RedirectToAction("Details", new { id = model.TrademarkId });
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpGet, Process(Name = "Trademarks.Details")]
        public ActionResult Details(int id)
        {
            var model = new TrademarkDetailsViewModel();

            try
            {
                // Create the query
                var query = new TrademarkQuery
                {
                    TrademarkIds = new List<int> { id },
                    Envelope = "full"
                };

                // Get the category
                var result = _metadataServiceClient.GetTrademarks(query).Result.FirstOrDefault();

                model.Trademark = new TrademarkViewModel
                {
                    TrademarkId = result.TrademarkId,
                    Name = result.Name,
                    Parent = _metadataServiceClient.GetTrademarks(new TrademarkQuery
                    {
                        TrademarkIds = new List<int> { result.ParentId },
                        Envelope = "full"
                    }).Result.FirstOrDefault(),
                    Children = result.Children,
                    LongDescription = result.LongDescription,
                    ShortDescription = result.ShortDescription,
                    Code = result.Code,
                    ERPCode = result.ERPCode,
                    Products = result.Products,
                    Properties = result.Properties,
                    Status = result.Status,
                    Tags = result.Tags,
                    Pictures = result.Pictures
                };

                model.Trademark = this.CreateSelectLists(model.Trademark);

                // Get the products
                var productIds = new List<int>();

                // Create the product query
                var productQuery = new ProductQuery()
                {
                    TrademarkIds = new List<int> { id }
                };

                // Get the products
                model.Products = _metadataServiceClient.GetProducts(productQuery).Result;

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Markalar",
                    Link = "/trademarks"
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
                Log(LogMode.Error, $"There is an error while getting the trademark details! TrademarkId:{id}", ex);

                return View();
            }
        }

        [HttpPost, Process(Name = "Trademarks.Details")]
        public ActionResult Details(TrademarkViewModel model)
        {
            try
            {
                var command = new TrademarkCommand()
                {
                    TrademarkId = model.TrademarkId,
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Code = model.Code,
                    ERPCode = model.ERPCode,
                    ShortDescription = model.ShortDescription,
                    LongDescription = model.LongDescription,
                    SortOrder = model.SortOrder,
                    Status = model.Status,
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId
                };

                var response = _metadataServiceClient.UpdateTrademark(command);
            }
            catch (Exception e)
            {
                Log(LogMode.Error, $"There is an error while updating the trademark details! TrademarkId:{model.TrademarkId}", e);

                return RedirectToAction("Details", new { id = model.TrademarkId });
            }

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Markalar",
                Link = "/trademarks"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = model.Name
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return RedirectToAction("Details", new { id = model.TrademarkId });
        }

        private TrademarkViewModel CreateSelectLists(TrademarkViewModel model)
        {
            var parentTrademarks = _metadataServiceClient.GetTrademarks(new TrademarkQuery()).Result;
            var categoryStatus = new Dictionary<Headstone.Framework.Models.EntityStatus, string>
            {
                { Headstone.Framework.Models.EntityStatus.Active, "Aktif" },
                { Headstone.Framework.Models.EntityStatus.Passive, "Pasif" }
            };
            var parentSelectList = parentTrademarks.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.TrademarkId.ToString()
            }).ToList();

            parentSelectList.Add(new SelectListItem()
            {
                Text = "Hiçbiri",
                Value = "",
                Selected = true
            });

            model.ParentList = parentSelectList;
            model.Statuses = new SelectList(categoryStatus.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");

            return model;
        }

        #region [ Data functions ]

        [Process(Name = "Trademarks.GetTrademarks")]
        public JsonResult GetTrademarks([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var query = new TrademarkQuery();

                var result = _metadataServiceClient.GetTrademarks(query);
                var response = result.Result.Select(r => new TrademarkViewModel
                {
                    TrademarkId = r.TrademarkId,
                    Name = r.Name,
                    ShortDescription = r.ShortDescription,
                    LongDescription = r.LongDescription,
                    Code = r.Code,
                    ERPCode = r.ERPCode,
                    Products = r.Products
                }).ToList().OrderByDescending(i => i.TrademarkId).ToDataSourceResult(req);

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the trademark data!", ex);
                
                return Json(ex);
            }
        }

        #endregion
    }
}