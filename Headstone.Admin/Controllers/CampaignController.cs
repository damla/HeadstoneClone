using Headstone.Admin.Models.ViewModels;
using Headstone.AI.Controllers;
using Headstone.Framework.Models;
using Headstone.Service;
using Headstone.Service.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.Admin.Controllers
{
    [RoutePrefix("campaigns")]
    public class CampaignController : BaseController
    {
        private CampaingService campaignService = new CampaingService();
        
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("{CampaignID}")]
        public ActionResult Details(int CampaignID)
        {
            return View();
        }

        [Route("new")]
        public ActionResult New()
        {
            return View();
        }
        [Route("new")]
        [HttpPost]
        public ActionResult New(int CampaignID)
        {
            return View();
        }

        [Process(Name = "Campaings.GetGetCampaigns")]
        public JsonResult GetCampaigns([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var orders = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
                {
                }).Result;

                var result = orders.Select(a => new CampaignViewModel()
                {
                    CampaignID = a.CampaignID,
                    DiscountAmount = a.DiscountAmount,
                    DiscountType = a.DiscountType,
                    LongDescription = a.LongDescription,
                    Name = a.Name,
                    RelatedDataEntityID = a.RelatedDataEntityID,
                    RelatedDataEntityName = a.RelatedDataEntityName,
                    ShortDescription = a.ShortDescription
                }).ToList().ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the order data!", ex);

                return Json(ex);
            }
        }
    }
}