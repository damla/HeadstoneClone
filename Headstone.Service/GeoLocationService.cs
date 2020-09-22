using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Requests;
using Headstone.Service.Base;
using Headstone.Service.Interfaces;
using LinqKit;

namespace Headstone.Service
{
    public class GeoLocationService : IGeoLocationService
    {
        private GeoLocationServiceBase geoLocationServiceBase = new GeoLocationServiceBase();

        #region [ Queries ]

        public GeoLocationServiceResponse<GeoLocation> GetGeoLocations(GeoLocationRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new GeoLocationServiceResponse<GeoLocation>();

            var geoLocations = new List<GeoLocation>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<GeoLocation, bool>> filterPredicate = PredicateBuilder.New<GeoLocation>(true);

            // Add the filters
            if (req.GeoLocationIds.Any())
            {
                filterPredicate = filterPredicate.And(r => req.GeoLocationIds.Contains(r.GeoLocationId));
            }
            if (req.GeoLocationPaths.Any())
            {
                filterPredicate = filterPredicate.And(r => req.GeoLocationPaths.Contains(r.Path));
            }
            if (req.GeoLocationParents.Any())
            {
                filterPredicate = filterPredicate.And(r => req.GeoLocationParents.Contains(r.Parent));
            }
            if (!String.IsNullOrEmpty(req.Prefix))
            {
                filterPredicate = filterPredicate.And(r => r.Name.StartsWith(req.Prefix));
            }
            try
            {
                // Make the query
                if (filterPredicate.Parameters.Count > 0)
                {
                    geoLocations = geoLocationServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
                }
                else
                {
                    geoLocations = geoLocationServiceBase.GetAllIncluding(includes.ToArray()).Result;
                }
            }
            catch (Exception e)
            {
                throw;
            }

            response.Type = ServiceResponseTypes.Success;
            response.Result = geoLocations;

            return response;
            #endregion
        }

        #endregion
    }
}



