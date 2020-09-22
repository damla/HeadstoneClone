using Headstone.AI.Models.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    public class HomeController : BaseController
    {
        [Route("")]
        [Route("dashboard")]
        public ActionResult Index()
        {
            var model = new DashboardViewModel()
            {
                CurrentCulture = CurrentCulture,
                CurrentUser = CurrentUser,
            };

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ana sayfa"                
            });
                      
            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View(model);
        }        
    }
}
// admin giris
// kullanici adi: ekin.barut@tgworkshop.com
// sifre: 123123