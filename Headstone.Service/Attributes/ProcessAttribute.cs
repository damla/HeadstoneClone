using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Headstone.Service.Attributes
{
    public class ProcessAttribute: ActionFilterAttribute
    {
        public string Name { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                filterContext.HttpContext.Items.Add("Process", Name);
            }
        }
    }
}
