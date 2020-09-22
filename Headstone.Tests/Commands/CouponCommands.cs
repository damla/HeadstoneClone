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
    [TestClass]
    public class CouponCommands
    {
        // TODO: DONE

        private CouponService couponService = new CouponService();

        [TestMethod]
        public void CreateCoupon()
        {
            var couponCommand = new CouponCreated()
            {
                CouponCode = "TESTCOUPON",
                OwnerID = 1010,
                CampaignID = 8,
                FromDate = DateTime.Now
            };
            var response = couponService.CreateCoupon(couponCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateCoupon()
        {
            var couponCommand = new CouponUpdated()
            {
                CouponID = 3,
                CouponCode = "Bendensin",
                FromDate = DateTime.Now,
                Status = Framework.Models.EntityStatus.Draft
            };

            var response = couponService.UpdateCoupon(couponCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateCouponStatus()
        {
            var couponCommand = new CouponUpdated()
            {
                CouponID = 3,
                Status = Framework.Models.EntityStatus.Passive
            };

            var response = couponService.UpdateCouponStatus(couponCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteCoupon()
        {
            var couponCommand = new CouponDeleted()
            {
                CouponID = 5
            };
            var response = couponService.DeleteCoupon(couponCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }
    }
}
