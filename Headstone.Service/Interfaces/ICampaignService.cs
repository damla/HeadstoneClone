using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Campaign;
using Headstone.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Interfaces
{
    public interface ICampaignService
    {
        #region [ Queries ]

        CampaignServiceResponse<Campaign> GetCampaigns(CampaignRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ Campaign ]

        CampaignServiceResponse<Campaign> CreateCampaign(CampaignCreated ev, List<ServiceLogRecord> logRecords = null);

        CampaignServiceResponse<Campaign> UpdateCampaign(CampaignUpdated ev, List<ServiceLogRecord> logRecords = null);

        CampaignServiceResponse<Campaign> UpdateCampaignStatus(CampaignUpdated ev, List<ServiceLogRecord> logRecords = null);

        CampaignServiceResponse<Campaign> DeleteCampaign(CampaignDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}
