using Headstone.AI.Models.RequestModels;
using Headstone.AI.Models.ViewModels;
using Headstone.Service.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Models;
using Lidia.Identity.API.Models.Commands;
using Lidia.Identity.API.Models.Queries;
using Lidia.Identity.Common.Models;
using Lidia.Identity.SDK.Net.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator")]
    [RoutePrefix("roles")]
    public class RolesController : BaseController
    {
        private IdentityServiceClient _identityClient;

        public RolesController()
        {
            _identityClient = new IdentityServiceClient();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("new")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost, Route("new")]
        public ActionResult New(NewRoleRequest req)
        {
            var command = new RoleCommand
            {
                Name = req.Name,
                Description = "",
                SourceRoleCode = "",
                Status = req.Status? Lidia.Identity.Common.Models.EntityStatus.Active: Lidia.Identity.Common.Models.EntityStatus.Passive
            };

            var response = _identityClient.CreateRole(command);

            if (response.Type != Lidia.Identity.Common.Models.ServiceResponseTypes.Completed)
            {
                return RedirectToAction("New");
            }
            else
            {
                return View();
            }

        }

        [Route("{id}/edit")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        [Route("{id}")]
        public ActionResult Details(int id)
        {
            return View();
        }

        #region [ Data functions ]

        [Process(Name = "Roles.GetAllRoles"), Route("getallroles")]
        public JsonResult GetAllRoles([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var command = new RoleQuery
                {
                    
                };

                var response = _identityClient.GetRoles(command);

                var result = response.Result.Select(res => new RoleViewModel
                {
                    Id = res.Id,
                    Name = res.Name,
                    Desc = res.Description
                }).ToList().OrderByDescending(i => i.Id).ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log(LogMode.Error, $"There is an error while getting the role data!", e);

                var model = new RoleViewModel();

                return Json(model);
            }
        }

        #endregion
    }
}