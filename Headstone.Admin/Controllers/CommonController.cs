using Headstone.AI.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    public class CommonController : BaseController
    {
        public ActionResult Header()
        {
            var model = new HeaderViewModel()
            {
                CurrentCulture = CurrentCulture,
                CurrentUser = CurrentUser,
            };

            return PartialView(model);
        }

        public ActionResult Sidebar()
        {
            return PartialView();
        }

        public ActionResult RightSidebar()
        {
            return PartialView();
        }

        public ActionResult Footer()
        {
            return PartialView();
        }

        public ActionResult Breadcrumb(List<BreadcrumbItemViewModel> breadcrumb = null)
        {
            var model = new HeaderViewModel();

            model.Breadcrumb = breadcrumb;
            return PartialView(model);
        }
    }
}