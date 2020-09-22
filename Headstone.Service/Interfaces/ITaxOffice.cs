using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Interfaces
{
    public interface ITaxOffice
    {
        TaxOfficeServiceResponse<TaxOffice> GetOffices(TaxOfficeRequest tax, List<ServiceLogRecord> logRecords = null);

    }
}
