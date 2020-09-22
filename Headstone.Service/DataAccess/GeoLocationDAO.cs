using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headstone.Models;
using Headstone.Framework.Data.Channels;

namespace Headstone.Service.DataAccess
{
    public class GeoLocationDAO : EFDataChannel<GeoLocation,HeadstoneDbContext>
    {

    }
}
