using Headstone.AI.Models.ViewModels;
using Headstone.Framework.Logging;
using Lidia.Identity.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Controllers
{
    [Authorize(Roles = "Administrator,Superuser")]
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

        #region [ Login and logout ]

        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string returnUrl)
        {
            // Add debug log
            //LogService.Debug($"Getting the login page. SessionId:{SessionId}");

            // Create the model
            var model = new LoginViewModel()
            {
            };

            // Set the return url into the viewbag
            ViewBag.ReturnUrl = returnUrl;

            //LogService.Debug("Login view model created.", "LOGIN");

            return View(model);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        [Route("login")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // Add debug log
            //LogService.Debug($"Getting the login page (HTTP_POST). SessionId:{SessionId}");

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:

                    // Return success
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.Remember });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid credentials!");
                    model.Errors.Add("Hatalı kullanıcı adı veya şifre !");

                    return View(model);
            }
        }

        [Route("logoff")]
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

        #region [ Password ]

        [Route("forgot-password")]
        public ActionResult ForgotPassword()
        {
            // Add debug log
            LogService.Debug($"Getting the forgot password page. SessionId:{SessionId}");

            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        [Route("forgot-password")]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            LogService.Debug($"Getting the forgot password page (HTTP_POST). SessionId:{SessionId}");

            // Check the incoming email address
            var user = UserManager.FindByEmail(model.Email);
                        
            if (user == null)
            {
                model.HasErrors = true;
                model.Result = "Email not found!";

                // Add debug log
                LogService.Debug($"Could not found the user. Email:{model.Email}; SessionId:{SessionId}");
            }
            else
            {
                // Add debug log
                LogService.Debug($"Found the user.UserId:{user.Id}; Email:{user.Email}; SessionId:{SessionId}");

                // Creat the password reset link
                var token = UserManager.GeneratePasswordResetToken(user.Id);

                // Create the reset password notification view model
                var notificationViewModel = new ResetPasswordNotificationViewModel()
                {
                    SiteRoot = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "/"),
                    User = user,
                    ResetToken = HttpUtility.UrlEncode(token),
                    SecurityStamp = user.SecurityStamp
                };
                
                #region [ Reset password email ]

                //// Create the notification
                //var notificationBody = ViewToString("~/Views/Templates/Email/PasswordReset.cshtml", notificationViewModel);

                //// Send confirmation email
                //var notificationResponse = notificationServiceClient.SendNotification(new NotificationCommand()
                //{
                //    appkey = ConfigurationManager.AppSettings["AppKey"],
                //    env = ConfigurationManager.AppSettings["Environment"],
                //    ut = UserToken,
                //    sid = SessionId,
                //    uid = UserId,
                //    t = 1, // Email
                //    fn = "Zippsi",
                //    fa = "m@ntf.zippsi.com",
                //    s = "Şifre güncelleme",
                //    b = notificationBody,
                //    rt = 1,
                //    rcp = new List<RecipientCommand>()
                //    {
                //        new RecipientCommand()
                //        {
                //            an = user.Firstname + " " + user.Lastname,
                //            adr = user.Email
                //        }
                //    }
                //});

                //if (notificationResponse.Type != ServiceResponseTypes.Success)
                //{
                //    // Add debug log
                //    LogService.Debug($"Could not sent password recovery mail sent.UserId:{user.Id}; Email:{user.Email}; SessionId:{SessionId}; Errors:{String.Join(",", notificationResponse.Errors)}");

                //    model.HasErrors = true;
                //    model.Result = "";
                //}

                //// Add debug log
                //LogService.Debug($"Password recovery mail sent.UserId:{user.Id}; Email:{user.Email}; SessionId:{SessionId}");

                #endregion

                model.Result = "Password recovery email sent!";

            }

            return View(model);
        }

        [Route("reset-password")]
        public ActionResult ResetPassword(string t)
        {
            // Add debug log
            LogService.Debug($"Getting the reset password page. SessionId:{SessionId}");

            var model = new ResetPasswordViewModel()
            {
                Token = t
            };

            // Split the token
            string[] arrToken = t.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            // Set the required information
            var passwordUpdateToken = HttpUtility.UrlDecode(arrToken.ElementAtOrDefault(0));
            var securityStamp = arrToken.ElementAtOrDefault(1);

            // Add debug log
            LogService.Debug($"Password and security stamps parsed. SessionId:{SessionId}");

            if (securityStamp != null)
            {
                // Add debug log
                LogService.Debug($"Getting the user using the security stamp. SessionId:{SessionId}");

                // Try to get the user
                //var user = identityServiceClient.GetUsers(new UserQuery()
                //{
                //    ss = securityStamp
                //}).Result.FirstOrDefault();

                //if (user != null)
                //{
                //    // Add debug log
                //    LogService.Debug($"User found. Email:{user.Email}; SessionId:{SessionId}");

                //    // Set the emai
                //    model.Email = user.Email;
                //}
            }

            return View(model);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        [Route("reset-password")]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            // Add debug log
            LogService.Debug($"Getting the reset password page (HTTP_POST). SessionId:{SessionId}");

            if(model.Password != model.ConfirmPassword)
            {
                model.HasErrors = true;
                model.Result = "Passwords do not match!";

                return View(model);
            }

            // Split the token
            string[] arrToken = model.Token.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            // Set the required information
            var passwordUpdateToken = HttpUtility.UrlDecode(arrToken.ElementAtOrDefault(0));
            var securityStamp = arrToken.ElementAtOrDefault(1);

            // Correct the token
            passwordUpdateToken = passwordUpdateToken.Replace(" ", "+");

            // Add debug log
            LogService.Debug($"Password and security stamps parsed. SessionId:{SessionId}");

            if (securityStamp != null)
            {
                // Add debug log
                LogService.Debug($"Getting the user using the security stamp. SessionId:{SessionId}");

                // Try to get the user
                //var user = identityServiceClient.GetUsers(new UserQuery()
                //{
                //    ss = securityStamp
                //}).Result.FirstOrDefault();

                //if (user != null)
                //{
                //    // Set the context parameters for internal use
                //    HttpContext.Items.Add("SessionId", SessionId);
                //    HttpContext.Items.Add("UserToken", UserToken);

                //    // Change the password
                //    var passwordChangeResponse = UserManager.ResetPassword(user.Id, passwordUpdateToken, model.Password);

                //    if (passwordChangeResponse.Succeeded)
                //    {
                //        model.Result = DisplayMessages.UpdatePassword_PasswordUpdated;
                //        return View(model);
                //    }
                //}
            }

            model.HasErrors = true;
            model.Result = "Password could not be changed!";

            return View(model);
        }

        [Route("password-confirmation")]
        public ActionResult PasswordConfirmation()
        {
            return View();
        }

        #endregion

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
            return RedirectToAction("Index", "Home", new { area = "Admin" });
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