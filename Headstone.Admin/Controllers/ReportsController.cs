using Headstone.AI.Models.ViewModels;
using Headstone.Service.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Lidia.Identity.SDK.Net.Identity;
using Headstone.MetaData.Common.Models.Helpers;
using Headstone.MetaData.SDK.Net._452;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    [RoutePrefix("reports")]
    public class ReportsController : Controller
    {
        private IdentityServiceClient identityServiceClient = new IdentityServiceClient();
        private MetaDataServiceClient metaDataServiceClient = new MetaDataServiceClient();

        [Route("members")]
        public ActionResult Members()
        {
            return View();
        }

        [Route("products")]
        public ActionResult Products()
        {
            return View();
        }

        [Route("sales")]
        public ActionResult Sales()
        {
            return View();
        }

        [Route("orders")]
        public ActionResult Orders()
        {
            return View();
        }

        #region [ Data Functions ]

        [Process(Name = "Reports.GetMembers")]
        public JsonResult GetMembers([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                // Get the users
                var users = identityServiceClient.GetUsers(new Lidia.Identity.API.Models.Queries.UserQuery()).Result.ToList();

                var result = users.Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Email = u.Email,
                    Roles = u.Roles,
                    Created = u.Created,
                    Status = u.Status,
                }).ToList().OrderByDescending(u => u.Id).ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(e);
            }
        }

        [Process(Name = "Reports.GetProducts")]
        public JsonResult GetProducts([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                // Get the products
                var products = metaDataServiceClient.GetProducts(new Headstone.MetaData.API.Models.Queries.Live.ProductQuery()
                {
                    Envelope = "full"
                }).Result.ToList();

                #region [ Modifications ]

                // 
                 products.ForEach(p=>
                {
                    p.CategoryProducts.ForEach(a => a.Category.Products = null);
                    p.CategoryProducts.ForEach(b => b.Product = null);
                    p.TrademarkProducts.ForEach(t => t.Trademark.Products = null);
                    p.TrademarkProducts.ForEach(t => t.Product = null);
                    p.TrademarkProducts.ForEach(t => t.Trademark.Children = null);
                    p.Pictures.ForEach(s => s.Product = null);
                });

                #endregion

                // Get product categories
                var result = products.Select(p => new ProductViewModel()
                {
                    ProductId = p.ProductId,
                    Created = p.Created,
                    Name = p.Name,
                    Status = (Headstone.Framework.Models.EntityStatus)p.Status,
                    //CategoryList = String.Join(",",p.CategoryProducts.Select(a=>a.Category.Name)),
                    Pictures = p.Pictures,
                    TrademarkName = p.TrademarkProducts?.FirstOrDefault().Trademark.Name,
                }).ToList().OrderByDescending(p => p.ProductId);

                var model = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });

                return Json(model.ToDataSourceResult(req), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(e);
            }
        }

        public FileResult ExportFilter(List<int> ids, string entity)
        {
            var test = new byte[5];

            try
            {
                // Switch the entity
                switch (entity)
                {
                    case "members":
                        // Get the users
                        var users = identityServiceClient.GetUsers(new Lidia.Identity.API.Models.Queries.UserQuery()
                        {
                            RelatedUserIds = ids
                        }).Result.ToList();

                        var result = users.Select(u => new UserViewModel()
                        {
                            Id = u.Id,
                            Firstname = u.Firstname,
                            Lastname = u.Lastname,
                            Email = u.Email,
                            Created = u.Created,
                            Status = u.Status,
                        }).ToList();


                        ExcelHelper<UserViewModel> ee = new ExcelHelper<UserViewModel>();
                        XSSFWorkbook workbook = ee.GetWorkbook(result, "Üyeler");

                        //Write the workbook to a memory stream
                        MemoryStream output = new MemoryStream();
                        workbook.Write(output);

                        //Return the result to the end user

                        return File(output.ToArray(),   //The binary data of the XLS file
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Üye Listesi");

                    default:
                        return File(test.ToArray(),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Üye Listesi"); 
                }
            }
            catch (Exception ex)
            {

            }
            return File(test.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Üye Listesi");

        }


        #endregion
    }
}