using Headstone.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class ErrorController : BaseController
    {
        [Route("hata")]
        public ActionResult Error()
        {
            //ErrorViewModel model = new ErrorViewModel()
            //{
            //};

            //return View(model);
            return View();
        }
    }
}