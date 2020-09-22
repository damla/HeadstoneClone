using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Requests;

namespace Headstone.Service.Interfaces
{
    public interface IGeoLocationService
    {
        GeoLocationServiceResponse<GeoLocation> GetGeoLocations(GeoLocationRequest req, List<ServiceLogRecord> logRecords = null);
    }
}
