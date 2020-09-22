using Headstone.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common.Responses
{
    public class ServiceResponse<T>
    {
        public ServiceResponse()
        {
            PreProcessingTook = 0;
            ServiceTook = 0;
            TotalTook = 0;
            Source = ServiceResponseSources.Undefined;
            Result = new List<T>();
            Errors = new List<string>();
        }

        public List<T> Result { get; set; }

        public long PreProcessingTook { get; set; }

        public long ServiceTook { get; set; }

        public long TotalTook { get; set; }

        public ServiceResponseTypes Type { get; set; }

        public ServiceResponseSources Source { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public int ResultCount
        {
            get
            {
                return this.Result != null ? this.Result.Count() : 0;
            }
        }

        public int TotalCount { get; set; }

        public int CurrentPageIndex { get; set; }

        public List<string> Errors { get; set; }

        public List<ServiceLogRecord> LogRecords { get; set; }

        public ServiceResponse<T> InnerResponse { get; set; }
    }

    public class ServiceResponse
    {
        public ServiceResponse()
        {
            PreProcessingTook = 0;
            ServiceTook = 0;
            TotalTook = 0;
            Source = ServiceResponseSources.Undefined;
            Errors = new List<string>();
        }

        public long PreProcessingTook { get; set; }

        public long ServiceTook { get; set; }

        public long TotalTook { get; set; }

        public ServiceResponseTypes Type { get; set; }

        public ServiceResponseSources Source { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public List<string> Errors { get; set; }

        public List<ServiceLogRecord> LogRecords { get; set; }
    }
}
