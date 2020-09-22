using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.RequestModels
{
    public class NewRoleRequest
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public bool UserRead { get; set; }
        public bool UserWrite { get; set; }
        public bool ResellerRead { get; set; }
        public bool ResellerWrite { get; set; }
        public bool ReportRead { get; set; }
        public bool ReportWrite { get; set; }

    }
}