using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Coupon;
using Headstone.Models.Requests;
using System.Collections.Generic;

namespace Headstone.Service.Interfaces
{
    public interface ICouponService
    {
        #region [ Queries ]

        CouponServiceResponse<Coupon> GetCoupons(CouponRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ Coupon ]

        CouponServiceResponse<Coupon> CreateCoupon(CouponCreated ev, List<ServiceLogRecord> logRecords = null);

        CouponServiceResponse<Coupon> UpdateCoupon(CouponUpdated ev, List<ServiceLogRecord> logRecords = null);

        CouponServiceResponse<Coupon> UpdateCouponStatus(CouponUpdated ev, List<ServiceLogRecord> logRecords = null);

        CouponServiceResponse<Coupon> DeleteCoupon(CouponDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}