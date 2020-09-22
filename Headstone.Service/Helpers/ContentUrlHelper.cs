using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Headstone.Service.Helpers
{
    public static class ContentUrlHelper
    {
        public static string Static (this UrlHelper helper, string relativePath)
        {
            // Get the content root parameter
            var staticContentRoot = ConfigurationManager.AppSettings["BaseStaticUrl"];

            if (String.IsNullOrEmpty(relativePath)) return staticContentRoot;

            if (string.IsNullOrEmpty(staticContentRoot))
            {
                return UrlHelper.GenerateContentUrl(relativePath, helper.RequestContext.HttpContext);
            }

            if (relativePath.StartsWith("~"))
            {
                relativePath = relativePath.Substring(1);
            }

            if (staticContentRoot.EndsWith("/"))
            {
                staticContentRoot = staticContentRoot.Substring(0, staticContentRoot.Length - 1);
            }

            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }

            return staticContentRoot + relativePath;
        }
    }
}
