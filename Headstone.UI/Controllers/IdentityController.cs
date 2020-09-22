using Headstone.UI.Models.ViewModels;
using Headstone.Framework.Logging;
using Lidia.Identity.API.Models.Commands;
using Lidia.Identity.API.Models.Queries;
using Lidia.Identity.SDK.Net.Identity.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Headstone.UI.Controllers
{
    public class IdentityController : BaseController
    {

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

        #region 

        [HttpGet, Route("uye-ol")]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();

            return PartialView("_RegisterPopup", model);
        }

        [HttpPost, Route("uye-ol")]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //check if user exists
                var user = identityServiceClient.GetUserByEmail(model.Email).Result?.FirstOrDefault();

                if (user == null)
                {
                    var userCommand = new UserCommand()
                    {
                        Email = model.Email,
                        PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password),
                        Firstname = model.Firstname,
                        Lastname = model.Lastname,
                        UserName = model.Email,
                        EmailConfirmed = false,
                        Gender = model.Gender,
                        MobileNumberConfirmed = false,
                        DateOfBirth = string.Empty,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Status = Lidia.Identity.Common.Models.EntityStatus.Active,
                        AccountStatus = Lidia.Identity.Common.Models.AccountStatus.Active,
                        Environment = this.Environment,
                        UserToken = this.UserToken,
                        SessionId = this.SessionId,
                        Country = "TR",
                        RoleId = 1002, //User
                        ThreadId = ThreadId
                    };

                    var identityServiceResponse = identityServiceClient.CreateUser(userCommand);

                    if (identityServiceResponse.Type == Lidia.Identity.Common.Models.ServiceResponseTypes.Success)
                    {
                        var login = SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);

                        return Redirect("/");
                    }
                }
                else
                {
                    model.Errors.Add("Kullanıcı kaydı bulunuyor.");
                }

            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region [ Login and logout ]
        [AllowAnonymous, Route("giris")]
        public ActionResult Login() //string returnUrl
        {
            // Add debug log
            //LogService.Debug($"Getting the login page. SessionId:{SessionId}");

            // Create the model
            var model = new LoginViewModel()
            {
            };

            // Set the return url into the viewbag
            // ViewBag.ReturnUrl = returnUrl;

            //LogService.Debug("Login view model created.", "LOGIN");

            return PartialView("_LoginPopup"); // model
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken, Route("giris")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid) // dogru sekilde post etme/etmeme durumu
            {
                var result = SignInStatus.Failure; // signinstatus => enum 

                var test = identityServiceClient.GetUserByEmail(model.Email);

                var user = identityServiceClient.GetUserByEmail(model.Email).Result?.FirstOrDefault(); // microservice => maili kontrol ediyor varsa useri donduruyor

                if (user != null)
                {
                    if (user.Status == Lidia.Identity.Common.Models.EntityStatus.Active) // yonetim panelinde aktifse gibi
                    {
                        // This doesn't count login failures towards account lockout
                        // To enable password failures to trigger account lockout, change to shouldLockout: true
                        result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false); // giris yapiyor
                    }
                    else if (user.Status == Lidia.Identity.Common.Models.EntityStatus.Deleted) // silinmisse 
                    {
                        model.Errors.Add("Hesabınız silinmiştir. Lütfen destek birimi ile iletişime geçiniz.");

                        return View("~/Views/Home/Index.cshtml", model);
                    }

                    if (!user.EmailConfirmed) // ilk login gibi dusun 
                    {
                        var userResponse = identityServiceClient.UpdateUser(new UserCommand()
                        {
                            EmailConfirmed = true,
                            UserId = user.Id,
                            RelatedUserId = user.Id,
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            Gender = user.Gender,
                            UserName = user.Email,
                            CitizenId = user.CitizenId,
                            CitizenIdConfirmed = user.CitizenIdConfirmed,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                            MobileNumber = user.MobileNumber,
                            MobileNumberConfirmed = user.MobileNumberConfirmed,
                            Occupation = user.Occupation,
                            PasswordHash = user.PasswordHash,
                            SecurityStamp = user.SecurityStamp,
                            AccountStatus = user.AccountStatus,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            City = user.City,
                            Country = user.Country,
                            Status = user.Status,
                            DateOfBirth = user.DateOfBirth.HasValue ? user.DateOfBirth.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") : string.Empty,
                            Environment = this.Environment,
                            UserToken = this.UserToken,
                            SessionId = this.SessionId,
                            UserIP = HttpContext.GetClientIP()
                        });

                        if (userResponse.Type != Lidia.Identity.Common.Models.ServiceResponseTypes.Success) // gelen response success degilse logluyor
                        {
                            Log(Headstone.Framework.Models.LogMode.Error, $"There is an error while updating the user email confirmed ! Errors: {String.Join(",", userResponse.Errors)} UserId: {user.Id}", null);

                            //model.Errors.Add("Kullanıcı güncellenirken hata oluştu");

                            //return View("~/Views/Home/Index.cshtml", model);
                        }
                    }
                }

                switch (result)
                {
                    case SignInStatus.Success:

                        // Return success
                        return RedirectToLocal(returnUrl); // hangi urldeyse oraya don 

                    case SignInStatus.LockedOut:
                        return View("Lockout");

                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid credentials!");
                        model.Errors.Add("Hatalı kullanıcı adı veya şifre !");

                        return View("~/Views/Home/Index.cshtml", model);
                }
            }
            else
            {
                return View("~/Views/Home/Index.cshtml", model);
            }
        }

        [Route("cikis")]
        public ActionResult LogOff()
        {
            // Add debug log
            LogService.Debug($"Getting the logoff page. SessionId:{SessionId}");

            AuthenticationManager.SignOut();

            // Clear the session values
            Session.Clear();

            return Redirect("/");
        }

        #endregion

        [Route("sifremi-unuttum")]
        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();

            return View(model);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken] // attack onlemi 
        [Route("sifremi-unuttum")]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the user
                var user = identityServiceClient.GetUsers(new UserQuery()
                {
                    Emails = new List<string> { model.Email }
                }).Result.FirstOrDefault();

                if (user == null)
                {
                    model.Errors.Add("Bu e-posta adresine ait kullanıcı bulunamadı.");

                    return View(model);
                }
                else
                {
                    #region [ Site Root ]

                    // Get the current domain (it can be development, staging or production)
                    var siteRoot = String.Format("{0}://{1}{2}",
                                            System.Web.HttpContext.Current.Request.Url.Scheme,
                                            System.Web.HttpContext.Current.Request.Url.Host,
                                            System.Web.HttpContext.Current.Request.Url.Port == 80 ? string.Empty : ":" + System.Web.HttpContext.Current.Request.Url.Port);

                    #endregion
                }
                // mail yollanacak
                model.Result.Add("Şifre sıfırlama maili e-postanıza gönderildi.");
            }

            return View(model);
        }

        [Route("sifre-sifirla")]
        public ActionResult ResetPassword(string t, string email) // t guvenlik icin yaratilan guid
        {
            // Create the model
            var model = new ResetPasswordViewModel();

            // Get the user 
            var user = identityServiceClient.GetUsers(new UserQuery()
            {
                Emails = new List<string> { email }
            }).Result.FirstOrDefault();

            model.Email = email;
            model.Token = t;

            return View(model);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        [Route("sifre-sifirla")]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    model.Errors.Add("Şifreler eşleşmiyor");

                    return View(model);
                }

                // Get the user 
                var user = identityServiceClient.GetUsers(new UserQuery()
                {
                    Emails = new List<string> { model.Email }
                }).Result.FirstOrDefault();

                if (user == null)
                {
                    model.Errors.Add("Kullanıcı bulunamadı");

                    return View(model);
                }

                var response = UserManager.ResetPasswordAsync(user.Id, model.Token, model.Password);

                if (response.Result.Succeeded)
                {
                    var result = new LoginViewModel();

                    ViewBag.Message = "Password updated";

                    return View("~/Views/Home/Index.cshtml", result);
                }
            }

            return View(model);
        }

        [Route("sifre-onayla")]
        public ActionResult PasswordConfirmation()
        {
            return View();
        }


        #region [ Helper methods ]

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}