using Headstone.Framework.Configuration;
using Headstone.Framework.Logging;
using Headstone.Framework.Models;
using Headstone.Framework.Models.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using Headstone.Framework.Models.Configuration;
using Headstone.Models;
using Headstone.Service;
using Headstone.Models.Requests;
using System.Web.SessionState;

namespace Headstone.UI
{
    public class HeadstoneApplication : System.Web.HttpApplication
    {
        private static string _environment = ConfigurationManager.AppSettings["Environment"];
        public static List<GeoLocation> GeoLocations = new List<GeoLocation>();
        public static List<ConfigRecord> Configuration = new List<ConfigRecord>();

        private GeoLocationService _geolocationService = new GeoLocationService();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Load the configuration
            ConfigurationService.Load();

            // Set the application wide configuration parameters
            Configuration = ConfigurationService.Records;

            //Log the application initialization
            LogService.Debug("Headstone.UI - Application initialized");

            // Load the geolocations
            Task.Factory.StartNew(() =>
            {
                var response = _geolocationService.GetGeoLocations(new GeoLocationRequest()
                {
                    Environment = _environment,
                    SessionId = "x",
                    UserToken = "x",
                });

                GeoLocations = response.Result;
            });
        }

        protected void Application_Error()
        {
            // Get the last exception
            Exception exception = Server.GetLastError();
            Response.Clear();

            var httpException = exception as HttpException;

            string oldUrl = HttpContext.Current.Request.Url.AbsolutePath;

            // Try to extract data from the http context
            var userId = HttpContext.Current.Items.Contains("UserId") ? HttpContext.Current.Items["UserId"].ToString() : string.Empty;
            var threadId = HttpContext.Current.Items.Contains("ThreadId") ? HttpContext.Current.Items["ThreadId"].ToString() : string.Empty;
            var process = HttpContext.Current.Items.Contains("Process") ? HttpContext.Current.Items["Process"].ToString() : string.Empty;

            // Create the logrecord
            var logRecord = new LogRecord()
            {
                TenantId = "1",
                ApplicationId = "1",
                ApplicationIp = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"],
                UserId = userId,
                Level = LogMode.Error.ToString(),
                Process = process,
                ThreadId = threadId,
                Message = "Headstone.UI - Unhandled exception.",
                Exception = exception,
                //Data = data
            };

            // Log to the central logging system
            LogService.Log(logRecord);

            //Transfer to the error view
            Server.Transfer("~/Views/Error/Index.cshtml", true);
        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }
    }
}
