using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.UI.Models.ViewModels;

namespace Headstone.UI.Controllers
{
    public class StaticController : BaseController
    {
        // GET: Static
        public ActionResult Index()
        {
            // contact us gibi static sayfalar
            return View();
        }
        [Route("bize-ulasin")]
        public ActionResult ContactUs()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Bize Ulaşın"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }

        [Route("KVKK")]
        public ActionResult Agreement()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "KVKK Bilgilendirme"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }


        [Route("uyelik-sozlesmesi")]
        public ActionResult MembershipConditions()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Üyelik Sözleşmesi"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();
        }
    }
}