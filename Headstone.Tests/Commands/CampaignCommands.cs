using System;
using System.Collections.Generic;
using System.Linq;
using Headstone.Common;
using Headstone.Models;
using Headstone.Models.Events.Basket;
using Headstone.Models.Events.Campaign;
using Headstone.Models.Events.Coupon;
using Headstone.Service;
using Headstone.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Headstone.Tests.Commands
{
    // TODO: DONE
    [TestClass]
    public class CampaignCommands
    {
        private CampaingService campaingService = new CampaingService();

        [TestMethod]
        public void CreateCampaign()
        {
            var campaignCommand = new CampaignCreated()
            {
                RelatedDataEntityName = "CreateTestEntityName",
                RelatedDataEntityID = 1,
                Name = "CreateTestName",
                ShortDescription = "CreateTestSDescription",
                LongDescription = "CreateTestLDescription",
                DiscountType = DiscountType.Percentage,
                DiscountAmount = 30,
                Status = Framework.Models.EntityStatus.Active 
            };

            var response = campaingService.CreateCampaign(campaignCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void CreateCampaignProperty()
        {

            var campaignPropCommand = new CampaignPropertyCreated()
            {
                Key = "testKey",
                Value = "testValue",
                CampaignID = 14,
                Environment = "testEnvironment",
                ApplicationIP = "testAppId",
                UserToken = "testUserToken",
                SessionId = "testSessionId"
            };

            var response = campaingService.CreateCampaignProperty(campaignPropCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateCampaign()
        {
            var campaignCommand = new CampaignUpdated()
            {
                CampaignID = 15,
                RelatedDataEntityName = "UpdateTestEntityName",
                RelatedDataEntityID = 2,
                Name = "UpdateTestName",
                ShortDescription = "UpdateTestSDescription",
                LongDescription = "UpdateTestLDescription",
                DiscountType = DiscountType.Amount,
                DiscountAmount = 20
            };

            var response = campaingService.UpdateCampaign(campaignCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateCampaignStatus()
        {
            var campaignCommand = new CampaignUpdated()
            {
                CampaignID = 15,
                Status = Framework.Models.EntityStatus.Passive
            };

            var response = campaingService.UpdateCampaignStatus(campaignCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }


        [TestMethod]
        public void DeleteCampaignProperty()
        {
            var campaignPropCommand = new CampaignPropertyDeleted()
            {
                PropertyId = 5,
                CampaignID = 8
            };

            var response = campaingService.DeleteCampaignProperty(campaignPropCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteCampaign()
        {
            var campaignCommand = new CampaignDeleted()
            {
                CampaignID = 14
            };

            var response = campaingService.DeleteCampaign(campaignCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }
    }
}
