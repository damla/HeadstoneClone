using Headstone.Models;
using Headstone.Models.Requests;
using Headstone.Service;
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

namespace Headstone.AI
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private static string _environment = ConfigurationManager.AppSettings["Environment"];
        public static List<GeoLocation> GeoLocations = new List<GeoLocation>();

        private GeoLocationService _geolocationService = new GeoLocationService();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Load the configuration
            ConfigurationService.Load();

            // Load the geolocations
            //Task.Factory.StartNew(() =>
            //{
            //    var response = _geolocationService.GetGeoLocations(new GeoLocationRequest()
            //    {
            //        Environment = _environment,
            //        SessionId = "x",
            //        UserToken = "x",
            //    });

            //    GeoLocations = response.Result;
            //});

            // Log the application initialization
            //LogService.Debug("Application initialized");            
        }
        protected void Application_Error()
        {
            // Get the last exception
            Exception exception = Server.GetLastError();

            var httpException = exception as HttpException;

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
                Message = "Headstone.AI - Unhandled exception.",
                Exception = exception,
                //Data = data
            };

            // Log to the central logging system
            //LogService.Log(logRecord);
        }
    }
}
