using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class CampaignController : BaseController
    {
        // GET: Campaign
        public ActionResult Index()
        {
            return PartialView("_CampaignPopup");
        }
    }
}