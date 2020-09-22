using Headstone.Framework.SaaS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Models.Requests
{
    public class BaseRequest
    {

        #region [ Tenant information ]

        public int? TenantId { get; set; }

        public Tenant Tenant { get; set; }

        #endregion

        #region [ Application information ]

        public string ApplicationIP { get; set; }

        public int? ApplicationId { get; set; }

        public Application Application { get; set; }

        #endregion

        #region [ Service identity ]

        public string ClientId { get; set; }

        public string AppKey { get; set; }

        public string Token { get; set; }

        public string Environment { get; set; }

        #endregion

        #region [ User identity ]

        public string UserToken { get; set; }

        public string SessionId { get; set; }

        public string UserIP { get; set; }

        #endregion

        public DateTime TimeStamp { get; set; }

        public string Envelope { get; set; }

        public List<Headstone.Framework.Models.EntityStatus> Status = new List<Headstone.Framework.Models.EntityStatus>();
    }
}
