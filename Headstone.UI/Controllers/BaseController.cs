using Headstone.Service;
using Headstone.UI.Models.ViewModels;
using Headstone.Framework.Logging;
using Headstone.Framework.Models;
using Headstone.Framework.Models.Logging;
using Lidia.Identity.API.Models;
using Lidia.Identity.API.Models.Queries;
using Lidia.Identity.SDK.Net.Identity;
using Headstone.MetaData.API.Models.Live;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Headstone.UI.Controllers
{
    public class BaseController : Controller
    {
        private static string _environment = ConfigurationManager.AppSettings["Environment"];


        protected IdentityServiceClient identityServiceClient = new IdentityServiceClient();

        #region [ User and session properties ]

        public User CurrentUser { get; set; }

        public string UserToken { get; set; }

        public string Environment
        {
            get
            {
                return _environment;
            }
        }

        public int UserId
        {
            get
            {
                return User.Identity.IsAuthenticated ? User.Identity.GetUserId<int>() : 0;
            }
        }

        public string SessionId
        {
            get { return Session.SessionID; }
        }

        public string ThreadId { get; set; }

        public string SiteRoot { get; set; }

        #endregion

        #region [ Culture and Location ]

        public string CurrentLocation { get; private set; }

        public string CurrentCulture { get; private set; }

        #endregion

        static BaseController() { }

        public BaseController()
        {
            // Set the current culture
            CurrentCulture = "tr";
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            #region [ User ]
            if(requestContext.HttpContext.Session != null)
            {

            // Try to get user information from the session
            var user = requestContext.HttpContext.Session["User"] as User;

            if (user == null)
            {
                // Get the user id
                var userId = User.Identity.GetUserId<int>();

                if (userId != 0)
                {
                    // Get the contact information from the related service
                    user = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>()
                            .FindById(userId);

                    // Store contact data in the session
                    Session["User"] = user;
                }
            }

            // Set the user
            CurrentUser = user;

            #endregion

            #region [ User token ]

            // Get the user token
            var userToken = string.Empty;

            // Try to get the user token
            var tokenCookie = requestContext.HttpContext.Request.Cookies["lid.token"];

            if (tokenCookie == null)
            {
                // Get the token from the identity server
                userToken = identityServiceClient.GetUserGuid(new GuidQuery() { }).Result.FirstOrDefault();

                // Set the cookie
                tokenCookie = new HttpCookie("lid.token", userToken);
                tokenCookie.Expires = DateTime.MaxValue;
                requestContext.HttpContext.Response.Cookies.Add(tokenCookie);
            }
            else
            {
                // Get the token from the 
                userToken = tokenCookie.Value;
            }

            // Set the token
            UserToken = userToken;
            ViewBag.UserToken = userToken;

            #endregion

            #region [ User Id ]

            // Set the user id
            ViewBag.UserId = UserId;

            // Add it to http context
            if (HttpContext.Items["UserId"] == null)
            {
                HttpContext.Items.Add("UserId", UserId);
            }

            #endregion

            #region [ Session Id ]

            // Set the session id
            ViewBag.SessionId = SessionId;

            #endregion

            #region [ ThreadId ]

            if (String.IsNullOrEmpty(ThreadId))
            {
                // Set the thread id
                ThreadId = CreateThreadId(8);

                // Add it to http context
                if (HttpContext.Items["ThreadId"] == null)
                {
                    HttpContext.Items.Add("ThreadId", ThreadId);
                }
            }

            #endregion

            #region [ Site Root ]

            // Get the current domain (it can be development, staging or production)
            //SiteRoot = String.Format("{0}://{1}{2}",
            //                        System.Web.HttpContext.Current.Request.Url.Scheme,
            //                        System.Web.HttpContext.Current.Request.Url.Host,
            //                        System.Web.HttpContext.Current.Request.Url.Port == 80 ? string.Empty : ":" + System.Web.HttpContext.Current.Request.Url.Port);
            SiteRoot = ConfigurationManager.AppSettings["BaseUrl"];


                #endregion
            }

        }

        #region [ Override JsonResult ]

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        #endregion

        #region [ Helper methods ]

        protected string ViewToString<T>(string controllerName, string viewPath, T model, bool useCache = false)
        {
            try
            {
                using (var writer = new StringWriter())
                {
                    // Create a new route data
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", controllerName);

                    // Create fake controller context
                    var fakeControllerContext = new ControllerContext(
                                                    new HttpContextWrapper(
                                                        new HttpContext(
                                                            new HttpRequest(null, "http://google.com", null),
                                                            new HttpResponse(null))), routeData, new FakeController());

                    // Create the razor engine
                    var razorViewEngine = new RazorViewEngine();
                    var razorViewResult = razorViewEngine.FindView(fakeControllerContext, viewPath, "", false);

                    // Create view context
                    var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), writer);

                    // Render view
                    razorViewResult.View.Render(viewContext, writer);

                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static IDictionary<int, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
                throw new ArgumentException("Enumeration type is expected.");

            var dictionary = new Dictionary<int, string>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                dictionary.Add(value, name);
            }

            return dictionary;

        }

        public void Log(LogMode level, string message, Exception exception)
        {
            // Get the process id from the http context
            var process = HttpContext.Items.Contains("Process") ? HttpContext.Items["Process"].ToString() : string.Empty;

            // Create the logrecord
            var logRecord = new LogRecord()
            {
                TenantId = "x",
                ApplicationId = "x",
                ApplicationIp = HttpContext.Request.ServerVariables["LOCAL_ADDR"],
                UserId = UserId.ToString(),
                Level = level.ToString(),
                Process = process,
                ThreadId = ThreadId,
                Message = message,
                Exception = exception,
                //Data = data
            };

            // Log to the central logging system
            LogService.Log(logRecord);
        }

        protected string CreateThreadId(int numberOfChars)
        {
            // Create a char base
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Create a random object
            var random = new Random();

            // Create alphanumeric code
            return new string(Enumerable.Repeat(chars, numberOfChars)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion

        #region [ Data Functions ]

        public string GetIDPath(Category cat)
        {
            if (cat.Parent == null)
            {
                return string.Empty;
            }
            else
            {
                var path = GetIDPath(cat.Parent);
                if (!string.IsNullOrEmpty(path)) path += "#";

                return path + cat.Parent.CategoryId;
            }
        }

        public string GetNamePath(Category cat)
        {
            if (cat.Parent == null)
            {
                return string.Empty;
            }
            else
            {
                var path = GetNamePath(cat.Parent);
                if (!string.IsNullOrEmpty(path)) path += " > ";

                return path + cat.Parent.Name;
            }
        }

        #endregion
    }

    #region [ Json serialization corrections ]

    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            // Check for a valid contxt
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            // Get the response object
            HttpResponseBase response = context.HttpContext.Response;

            // Check for a redirect request
            if (!String.IsNullOrEmpty(response.RedirectLocation)) return;

            // Set the content type
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            // Set the encoding
            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            // Set the serializer and writer
            var scriptSerializer = JsonSerializer.Create(this.Settings);
            using (var sw = new StringWriter())
            {
                scriptSerializer.Serialize(sw, this.Data);
                response.Write(sw.ToString());
            }
        }
    }

    #endregion

    public class FakeController : ControllerBase
    {
        protected override void ExecuteCore() { }
    }

}