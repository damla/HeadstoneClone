using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.AI.Models.ViewModels;
using Kendo.Mvc.Extensions;
using Headstone.Framework.Logging;
using Headstone.Framework.Models;
using Headstone.Service.Attributes;
using Lidia.Identity.SDK.Net.Identity;
using Lidia.Identity.API.Models.Queries;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("transactions")]
    public class TransactionsController : BaseController
    {
        private IdentityServiceClient _identityService;
        private Headstone.Service.TransactionService transactionService = new Service.TransactionService();

        public TransactionsController()
        {
            _identityService = new IdentityServiceClient();
        }

        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ödemeler"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        #region [ Data Functions ]

        [Process(Name = "Transactions.GetTransactions")]
        public JsonResult GetTransactions([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var transaction = transactionService.GetTransactions(new Headstone.Models.Requests.TransactionRequest()
                {

                }).Result;

                var result = transaction.Select(t => new TransactionViewModel()
                {
                    TransactionId = t.TransactionID,
                    TransactionDate = t.Created,
                    Amount = t.TotalPrice,
                    Account = t.CardNumber,
                    UserId = t.UserID,
                    Firstname = t.FirstName,
                    LastName = t.LastName
                }).ToList().ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the transaction data!", ex);
                return Json(ex);
            }
        }
        
        #endregion
    }
}