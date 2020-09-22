using Headstone.AI.Models.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Headstone.Framework.Common.Extensions;
using Headstone.Framework.Logging;
using Lidia.Identity.API.Models.Commands;
using Lidia.Identity.API.Models.Queries;
using Lidia.Identity.Common.Models;
using Lidia.Identity.SDK.Net.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.Models.Requests;
using Headstone.Service;
using Headstone.Framework.Models;
using Headstone.Service.Attributes;
using System.IO;
using System.Configuration;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
    public class UsersController : BaseController
    {
        private IdentityServiceClient _identityClient;
        private GeoLocationService _geoLocationService;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {

                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public UsersController()
        {
            _identityClient = new IdentityServiceClient();
            _geoLocationService = new GeoLocationService();
        }

        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Kullanıcılar"                
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View();            
        }

        [HttpGet, Process(Name = "Users.New")]
        public ActionResult New()
        {
            var model = new UserViewModel();

            model = CreateSelectLists(model);

            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Kullanıcılar",
                Link = "/users"
            });

            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Yeni"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Process(Name = "Users.New")]
        public ActionResult New(UserViewModel request)
        {
            try
            {
                var command = new UserCommand()
                {
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    Email = request.Email,
                    UserName = request.Email,
                    Gender = request.Gender,
                    RoleId = request.RoleId,
                    DateOfBirth = request.DateOfBirth.HasValue ? request.DateOfBirth.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                    PhoneNumber = request.PhoneNumber,
                    MobileNumber = request.MobileNumber,
                    PasswordHash = UserManager.PasswordHasher.HashPassword(request.Password),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    City = request.City,
                    AccountStatus = request.AccountStatus,
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId
                };

                var response = _identityClient.CreateUser(command);

                var userAddress = new UserAddressCommand
                {
                    RelatedUserId = response.UserId.GetValueOrDefault(),
                    City = request.City,
                    Country = "Turkey",
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId
                };

                var addressResponse = _identityClient.CreateUserAddress(userAddress);

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Kullanıcı",
                    Link = "/users"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Yeni"
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

                return RedirectToAction("Details", new { id = response.UserId });
            }
            catch (Exception e)
            {
                Log(LogMode.Error, "There is an error while creating a user!", e);

                // Create the model
                var model = new UserViewModel();

                // Get the locations
                model = CreateSelectLists(model);

                // Return the view
                return View(model);
            }

        }

        [HttpGet, Process(Name = "Users.Details")]
        public ActionResult Details(int id)
        {
            try
            {
                var model = new UserDetailViewModel();

                #region [ User Details ]

                //GET USER DETAILS
                var userQuery = new UserQuery()
                {
                    RelatedUserIds = new List<int> { id },
                    Envelope = "full"
                };

                // Get the user
                var userResponse = _identityClient.GetUsers(userQuery).Result.SingleOrDefault();

                // Create user model
                var userModel = new UserViewModel
                {
                    Id = userResponse.Id,
                    Firstname = userResponse.Firstname,
                    Lastname = userResponse.Lastname,
                    Email = userResponse.Email,
                    Username = userResponse.UserName,
                    PhoneNumber = userResponse.PhoneNumber,
                    MobileNumber = userResponse.MobileNumber,
                    Created = userResponse.Created,
                    Gender = userResponse.Gender,
                    DateOfBirth = userResponse.DateOfBirth,
                    AccountStatus = userResponse.AccountStatus,
                    SelectedStatus = (int)userResponse.AccountStatus,
                    City = MvcApplication.GeoLocations.FirstOrDefault(c=>c.Path == userResponse.City)?.Name,
                    Addresses = userResponse.Addresses,
                    BillingInfos = userResponse.BillingInfos,
                    Roles = userResponse.Roles
                };

                // Set the role id if there is one
                if (userResponse.Roles.Any())
                {
                    userModel.RoleId = userResponse.Roles.FirstOrDefault().RoleId;
                }

                // Create the selection lists
                userModel = CreateSelectLists(userModel);

                model.UserModel = userModel;

                // Load roles
                model.Roles = new List<RoleViewModel>();
                userResponse.Roles.Select(x => new RoleViewModel()
                {
                    Name = x.RoleName,
                    Desc = x.RoleDescription,
                    Code = x.SourceRoleCode,
                    Id = x.RoleId
                });

                #endregion

                #region [ User Data ]

                //Get user consent
                var consentModel = new List<UserConsentViewModel>();
                var consentQuery = new UserConsentQuery
                {
                    RelatedUserIds = new List<int> { id }
                };
                var consents = _identityClient.GetUserConsents(consentQuery).Result;
                model.Consents = consents.Select(x => new UserConsentViewModel
                {
                    ConsentType = x.ConsentType,
                    Approved = x.Approved,
                    Rejected = x.Rejected,
                    Revoked = x.Revoked,
                    ValidFrom = x.ValidFrom,
                    ValidUntil = x.ValidUntil,
                    UserIp = x.ApprovalIp
                }).ToList();

                #endregion

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Kullanıcılar",
                    Link = "/Users"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text =  userResponse.Firstname + " " + userResponse.Lastname
                });
                  
                ViewBag.Breadcrumb = breadcrumb;

                #endregion

                return View(model);
            }
            catch (Exception e)
            {
                Log(LogMode.Error, $"There is an error while getting the user details! UserId:{id}", e);

                throw;
            }
        }

        [HttpPost, Process(Name = "Users.Details")]
        public ActionResult Details(UserViewModel request)
        {

            try
            {
                // Load the existing user
                var existingUser = _identityClient.GetUsers(new UserQuery()
                {
                    RelatedUserIds = new List<int> { request.Id }
                }).Result.FirstOrDefault();

                if (existingUser == null)
                {

                }

                var command = new UserCommand()
                {
                    UserId = UserId,
                    RelatedUserId = request.Id,
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    RoleId = request.RoleId,
                    Gender = request.Gender,
                    UserName = request.Email,
                    CitizenId = existingUser.CitizenId,
                    CitizenIdConfirmed = existingUser.CitizenIdConfirmed,
                    Email = request.Email,
                    EmailConfirmed = existingUser.EmailConfirmed,
                    PhoneNumber = request.PhoneNumber,
                    PhoneNumberConfirmed = existingUser.PhoneNumberConfirmed,
                    MobileNumber = request.MobileNumber,
                    MobileNumberConfirmed = existingUser.MobileNumberConfirmed,
                    Occupation = existingUser.Occupation,
                    PasswordHash = (!String.IsNullOrEmpty(request.Password) ? UserManager.PasswordHasher.HashPassword(request.Password) : existingUser.PasswordHash),
                    SecurityStamp = existingUser.SecurityStamp,
                    AccountStatus = (Lidia.Identity.Common.Models.AccountStatus)request.SelectedStatus,
                    TwoFactorEnabled = existingUser.TwoFactorEnabled,
                    City = request.City,
                    Country = existingUser.Country,
                    Status = existingUser.Status,
                    DateOfBirth = request.DateOfBirth.HasValue ? request.DateOfBirth.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                    Environment = this.Environment,
                    UserToken = this.UserToken,
                    SessionId = this.SessionId,
                    UserIP = HttpContext.GetClientIP()
                };
                var response = _identityClient.UpdateUser(command);

                if (response.Type != Lidia.Identity.Common.Models.ServiceResponseTypes.Success)
                {
                    Log(LogMode.Error, $"There is an error while updating the user details! UserId:{request.Id}", null);

                    return RedirectToAction("Details", new { id = request.Id });
                }

                #region [ Breadcrumb ]

                // Create the breadcrumb
                var breadcrumb = new List<BreadcrumbItemViewModel>();
                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = "Kullanıcı",
                    Link = "/users"
                });

                breadcrumb.Add(new BreadcrumbItemViewModel()
                {
                    Text = request.Firstname + " " + request.Lastname                    
                });

                ViewBag.Breadcrumb = breadcrumb;

                #endregion

                return RedirectToAction("Details", new { id = request.Id });
            }
            catch (Exception e)
            {
                Log(LogMode.Error, $"There is an error while updating the user details! UserId:{request.Id}", e);

                return View();
            }

        }

        #region [ Data functions ]

        [Process(Name = "Users.GetUsers")]
        public JsonResult GetUsers([DataSourceRequest] DataSourceRequest req)
        {
            try
            {
                var command = new UserQuery()
                {
                    PageSize = req.PageSize,
                    PageIndex = req.Page
                };
                var response = _identityClient.GetUsers(command);

                var result = response.Result.Select(r => new UserViewModel
                {
                    Id = r.Id,
                    Firstname = r.Firstname,
                    Lastname = r.Lastname,
                    Email = r.Email,
                    Username = r.UserName,
                    Created = r.Created,
                    MobileNumber = r.MobileNumber,
                    DateOfBirth = r.DateOfBirth,
                    AccountStatus = r.AccountStatus,
                    Consents = new List<string>() { "DP", "MRK_SMS", "MRK_EMAIL" },
                    Roles = r.Roles
                }).ToList().OrderByDescending(i => i.Id).ToDataSourceResult(req);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log(LogMode.Error, $"There is an error while getting the user data!", ex);

                return Json(ex);
            }
        }

        #endregion

        #region [ Private functions ]

        private UserViewModel CreateSelectLists(UserViewModel model)
        {
            var accountStatuses = new Dictionary<Lidia.Identity.Common.Models.AccountStatus, string>
            {
                { Lidia.Identity.Common.Models.AccountStatus.Active, "Aktif"  },
                { Lidia.Identity.Common.Models.AccountStatus.Passive, "Pasif" },
                { Lidia.Identity.Common.Models.AccountStatus.Verified, "Onaylanmış" },
            };

            var genders = new Dictionary<string, string>();
            genders.Add("Erkek", "Erkek");
            genders.Add("Kadın", "Kadın");

            var cities = new Dictionary<string, string>();
            //cities.Add("İstanbul", "İstanbul");
            //cities.Add("Ankara", "Ankara");

            var request = new GeoLocationRequest();
            var response = _geoLocationService.GetGeoLocations(request).Result.Where(x => x.Parent == "TR").OrderBy(x => x.Name).ToList();
            foreach (var city in response)
            {
                cities.Add(city.Name, city.Name);
            }

            model.GenderList = new SelectList(genders.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text", model.Gender);
            model.AccountStatusList = new SelectList(accountStatuses.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text", (int)model.AccountStatus);
            model.CityList = new SelectList(cities.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text"); // EKIN

            // Load roles
            var roles = identityServiceClient.GetRoles(new RoleQuery() { }).Result;

            // Remove the not authorized role selections
            if (User.IsInRole("Superuser"))
            {
                roles.RemoveAll(r => r.Name == "Administrator");
            }
            else if (User.IsInRole("ResellerAdmin"))
            {
                roles.RemoveAll(r => r.Name == "Administrator");
                roles.RemoveAll(r => r.Name == "SuperUser");
            }
            else if (User.IsInRole("ResellerAgent"))
            {
                roles.RemoveAll(r => r.Name == "Administrator");
                roles.RemoveAll(r => r.Name == "SuperUser");
                roles.RemoveAll(r => r.Name == "ResellerAdmin");
            }

            // Set the selection list
            model.RoleList = new SelectList(roles.Select(x => new { Value = x.Id, Text = x.Name }), "Value", "Text", model.RoleId);

            return model;
        }

        #endregion
    }
}