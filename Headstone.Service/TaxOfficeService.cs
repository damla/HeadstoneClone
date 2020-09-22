using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Requests;
using Headstone.Service.Base;
using Headstone.Service.Interfaces;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service
{
    public class TaxOfficeService : ITaxOffice
    {
        private TaxOfficeServiceBase taxOfficeServiceBase = new TaxOfficeServiceBase();

        #region [ Queries ]

        public TaxOfficeServiceResponse<TaxOffice> GetOffices(TaxOfficeRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new TaxOfficeServiceResponse<TaxOffice>();

            var taxsApplications = new List<TaxOffice>();

            #region[ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<TaxOffice, bool>> filterPredicate = PredicateBuilder.New<TaxOffice>(true);

            // Add the filters
            if (req.TaxOfficesIds.Any())
            {
                filterPredicate = filterPredicate.And(t => req.TaxOfficesIds.Contains(t.TaxOfficeId));
            }
            #endregion

            try
            {
                // Make the query
                if (filterPredicate.Parameters.Count > 0)
                {
                    taxsApplications = taxOfficeServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
                }
                else
                {
                    taxsApplications = taxOfficeServiceBase.GetAllIncluding(includes.ToArray()).Result;
                }
            }
            catch (Exception e)
            {

                throw;
            }

            response.Type = ServiceResponseTypes.Success;
            response.Result = taxsApplications;

            return response;
        }

        #endregion

        #region[ Tax Office ]

        #endregion
    }
}
