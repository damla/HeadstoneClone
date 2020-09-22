using Headstone.MetaData.SDK.Net._452;
using Headstone.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class CommonController : BaseController
    {
        private MetaDataServiceClient metadataServiceClient = new MetaDataServiceClient();

        public ActionResult Header()
        {
            var model = new HeaderViewModel()
            {
                CurrentUser = CurrentUser
            };

            var categoryList = metadataServiceClient.GetCategories(new MetaData.API.Models.Queries.Live.CategoryQuery()
            {
            }).Result;

            model.Categories = categoryList.Select(c => new CategoryViewModel().From(c)).ToList();

            if (CurrentUser != null)
            {

                var favitemCount = identityServiceClient.GetUsers(new Lidia.Identity.API.Models.Queries.UserQuery()
                {
                    RelatedUserIds = new List<int> { CurrentUser.Id }
                }).Result.FirstOrDefault().Properties.Where(k=>k.Key=="FavProduct").Count();

                model.FavItemCount = favitemCount;
            }

            return PartialView("_Header", model);
        }

        public ActionResult Footer()
        {
            return PartialView("_Footer");
        }

        public ActionResult LeftSidebar()
        {
            return PartialView("_LeftSidebar");
        }
        public ActionResult Breadcrumb(List<BreadcrumbItemViewModel> breadcrumb = null)
        {
            var model = new HeaderViewModel();

            model.Breadcrumb = breadcrumb;
            return PartialView("_Breadcrumb", model);
        }
    }
}