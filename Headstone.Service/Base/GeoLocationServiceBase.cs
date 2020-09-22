using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headstone.Models;
using Headstone.Service.DataAccess;
using Headstone.Framework.Data.Services;

namespace Headstone.Service.Base
{
    class GeoLocationServiceBase : EFServiceBase<GeoLocation,GeoLocationDAO,HeadstoneDbContext>
    {
    }
}
