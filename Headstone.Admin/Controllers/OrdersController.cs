using Headstone.AI.Models.ViewModels;
using Headstone.Service.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Logging;
using Headstone.Framework.Models;
using Lidia.Identity.API.Models.Queries;
using Lidia.Identity.SDK.Net.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.Service;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("orders")]
    public class OrdersController : BaseController
    {
        private IdentityServiceClient _identityService;
        private OrderService orderService;

        public OrdersController()
        {
            _identityService = new IdentityServiceClient();
            orderService = new OrderService();
        }

        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Siparişler"                
            });
            
            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        [Process(Name = "Orders.Details")]
        public ActionResult Details(int id)
        {
            try
            { 
                var model = new OrderDetailViewModel();

                var userQuery = new UserQuery()
                {
                    RelatedUserIds = new List<int>() {  }
                };
                var user = _identityService.GetUsers(userQuery).Result.FirstOrDefault();
                var userModel = new UserViewModel()
                {
                    Id = user.Id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    City = user.City,
                    Addresses = user.Addresses
                };
                model.User = userModel;

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Siparişler",
                    Link = "/orders"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = ""
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

                var order = orderService.GetOrders(new Headstone.Models.Requests.OrderRequest()
                {
                    OrderIds = new List<int> { id }
                }).Result.FirstOrDefault();

                model.Order = order;

                return View(model);
            }
            catch (Exception e)
            {
                Log(LogMode.Error, $"There is an error while getting the order details! OrderId:{id}", e);

                throw;
            }
            
        }
        
        #region [ Data functions ]

        [Process(Name = "Orders.GetOrders")]
        public JsonResult GetOrders([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var orders = orderService.GetOrders(new Headstone.Models.Requests.OrderRequest()
                {
                }).Result;

                var result = orders.Select(a => new OrderViewModel()
                {
                    OrderId = a.OrderID,
                    Total = a.TotalPrice,
                    OrderDate = a.Created
                }).ToList().ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the order data!", ex);

                return Json(ex);
            }
        }
        
        #endregion
    }
}