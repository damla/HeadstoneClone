using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : Controller
    {
        [Route("product-configuration")]
        public ActionResult ProductConfiguration()
        {
            return View();
        }

        [Route("authorization")]
        public ActionResult Authorization()
        {
            return View();
        }
    }
}