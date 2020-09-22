using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Coupon;
using Headstone.Models.Requests;
using Headstone.Service.Base;
using Headstone.Service.Interfaces;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service
{
    public class CouponService : ICouponService
    {
        private CouponServiceBase _couponServiceBase = new CouponServiceBase();

        #region [ Queries ]

        public CouponServiceResponse<Coupon> GetCoupons(CouponRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new CouponServiceResponse<Coupon>();

            var Coupons = new List<Coupon>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<Coupon, bool>> filterPredicate = PredicateBuilder.New<Coupon>(true);

            // Add the filters
            if (req.CouponIDs.Any())
            {
                filterPredicate = filterPredicate.And(r => req.CouponIDs.Contains(r.CouponID));
            }

            if (req.OwnerIDs.Any())
            {
                filterPredicate = filterPredicate.And(m => req.OwnerIDs.Contains(m.OwnerID));
            }

            if (req.CampaignIDs.Any())
            {
                filterPredicate = filterPredicate.And(m => req.CampaignIDs.Contains(m.CampaignID));
            }

            #endregion

            // Make the query
            if (filterPredicate.Parameters.Count > 0)
            {
                Coupons = _couponServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
            }
            else
            {
                Coupons = _couponServiceBase.GetAllIncluding(includes.ToArray()).Result;
            }

            response.Type = Headstone.Common.ServiceResponseTypes.Success;
            response.Result = Coupons;

            return response;
        }

        #endregion

        #region [ Coupon ]

        public CouponServiceResponse<Coupon> CreateCoupon(CouponCreated ev, List<ServiceLogRecord> logRecords = null)
        {
            // Create the watch
            var sw = new Stopwatch();
            sw.Start();

            // Create a log record collection if necessary
            if (logRecords == null)
            {
                logRecords = new List<ServiceLogRecord>();
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Create Coupon request received."
            });

            var response = new CouponServiceResponse<Coupon>();

            #region [ Validate Request ]

            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming event data."
            });

            // Check required data
            List<string> dataErrors = new List<string>();

            //if (ev.ResellerId == default(int) || String.IsNullOrEmpty(ev.ResellerName))
            //{
            //    dataErrors.Add("No reseller!");
            //}

            //if (ev.Items == null || ev.Items.Count == 0)
            //{
            //    dataErrors.Add("No Coupon item(s)!");
            //}

            //if (String.IsNullOrEmpty(ev.Name))
            //{
            //    dataErrors.Add("No product name!");
            //}
            //if (ev.Items.Count == 0)
            //{
            //    dataErrors.Add("No Coupon item");
            //}

            if (dataErrors.Count > 0)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = dataErrors.Count + " error(s) found within the event data! Terminating the process. Errors:" + String.Join(";", dataErrors)
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "There are some errors with the incoming event data!";
                response.Errors.AddRange(dataErrors);
                response.LogRecords = logRecords;

                return response;
            }

            #endregion

            #region [ Data manuplation ]

            // Stop the timer
            sw.Stop();

            // Set the pre-processing time and start the time
            response.PreProcessingTook = sw.ElapsedMilliseconds;
            sw.Start();
            #endregion

            #region [ Create Object ]
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Creating Coupon."
            });

            // Create the reseller application
            var command = new Coupon()
            {
                CouponCode = ev.CouponCode,
                OwnerID = ev.OwnerID,
                CampaignID = ev.CampaignID,
                FromDate = ev.FromDate,
                ToDate = ev.ToDate,
                Status = ev.Status,
                Created = DateTime.Now
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Coupon created", ev.UserToken, ev.SessionId)
            });
            #endregion

            #region [ Save Coupon ]

            var baseServiceResponse = _couponServiceBase.Create(command);
            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving Coupon!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while saving the request!";
                response.Errors.AddRange(baseServiceResponse.Errors);
                response.LogRecords = logRecords;

                return response;

            }
            else
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = string.Format("Coupon successfuly created. CouponId:{0}",
                                            command.CouponID)
                });

                // Add the new object to the result
                response.Result.Add(command);

                // Set the object id
                response.CouponID = command.CouponID;
            }
            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Coupon successfuly created. CouponId:{0}",
                                            command.CouponID);
            response.LogRecords = logRecords;

            #endregion

            return response;
        }

        public CouponServiceResponse<Coupon> DeleteCoupon(CouponDeleted ev, List<ServiceLogRecord> logRecords = null)
        {
            // Create the watch
            var sw = new Stopwatch();
            sw.Start();

            // Create a log record collection if necessary
            if (logRecords == null)
            {
                logRecords = new List<ServiceLogRecord>();
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Coupon delete request received."
            });

            // Create a response object
            var response = new CouponServiceResponse<Coupon>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data
            List<string> dataErrors = new List<string>();

            if (ev.CouponID == default(int))
            {
                dataErrors.Add("No valid Coupon id found!");
            }

            if (dataErrors.Count > 0)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = dataErrors.Count + " error(s) found within the posted data! Terminating the process. Errors:" + String.Join(";", dataErrors)
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "There are some errors with the incoming request data!";
                response.Errors.AddRange(dataErrors);
                response.LogRecords = logRecords;

                return response;
            }

            #endregion


            #region [ Data manuplation ]


            #endregion


            // Stop the timer
            sw.Stop();

            // Set the pre-processing time and start the time
            response.PreProcessingTook = sw.ElapsedMilliseconds;
            sw.Start();


            #region [ Load reseller application ]


            Coupon Coupon = _couponServiceBase.Get(r => r.CouponID == ev.CouponID).Result.FirstOrDefault();

            if (Coupon == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Coupon found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Coupon found with the given id!";
                response.Errors.Add("No Coupon found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }
            #endregion

            #region [ Delete reseller application ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Deleting Coupon."
            });

            // Delete the billing info
            var baseServiceResponse = _couponServiceBase.Delete(Coupon);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the Coupon!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the Coupon!";
                response.Errors.Add("There was an error while deleting the Coupon!");
                response.Errors.AddRange(baseServiceResponse.Errors);
                response.LogRecords = logRecords;

                return response;

            }
            else
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = string.Format("Coupon successfuly deleted. CouponId:{0}",
                                            Coupon.CouponID)
                });

                // Add the new object to the result
                response.Result.Add(Coupon);

                // Set the role id
                response.CouponID = Coupon.CouponID;
            }

            #endregion


            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Coupon successfuly deleted. CouponId:{0}",
                                            Coupon.CouponID);
            response.LogRecords = logRecords;

            return response;
        }

        public CouponServiceResponse<Coupon> UpdateCoupon(CouponUpdated ev, List<ServiceLogRecord> logRecords = null)
        {
            // Create the watch
            var sw = new Stopwatch();
            sw.Start();

            // Create a log record collection if necessary
            if (logRecords == null)
            {
                logRecords = new List<ServiceLogRecord>();
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Coupon update request received."
            });

            // Create a response object
            var response = new CouponServiceResponse<Coupon>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data
            List<string> dataErrors = new List<string>();

            if (ev.CouponID == default(int))
            {
                dataErrors.Add("No valid Coupon id found!");
            }

            if (dataErrors.Count > 0)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = dataErrors.Count + " error(s) found within the posted data! Terminating the process. Errors:" + String.Join(";", dataErrors)
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "There are some errors with the incoming request data!";
                response.Errors.AddRange(dataErrors);
                response.LogRecords = logRecords;

                return response;
            }

            #endregion

            #region [ Data manuplation ]


            #endregion

            // Stop the timer
            sw.Stop();

            // Set the pre-processing time and start the time
            response.PreProcessingTook = sw.ElapsedMilliseconds;
            sw.Start();

            #region [ Load the reseller application ]

            Coupon Coupon = _couponServiceBase.Get(r => r.CouponID == ev.CouponID).Result.FirstOrDefault();

            if (Coupon == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Coupon found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Coupon found with the given id!";
                response.Errors.Add("No Coupon found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Coupon loaded."
            });

            // Update the reseller application
            Coupon.CouponCode = ev.CouponCode;
            Coupon.OwnerID = ev.OwnerID;
            Coupon.CampaignID = ev.CampaignID;
            Coupon.FromDate = ev.FromDate;
            Coupon.ToDate = ev.ToDate;
            Coupon.Status = ev.Status;
            Coupon.Updated = DateTime.Now;

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Coupon updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _couponServiceBase.Update(Coupon);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while updating the Coupon!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating Coupon!";
                response.Errors.AddRange(baseServiceResponse.Errors);
                response.LogRecords = logRecords;

                return response;
            }
            else
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = string.Format("Coupon successfuly updated. CouponId:{0}",
                                                Coupon.CouponID)
                });

                // Set the output information
                response.Result.Add(Coupon);
                response.CouponID = Coupon.CouponID;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Coupon successfuly updated. CouponId:{0}",
                                                Coupon.CouponID);
            response.LogRecords = logRecords;

            return response;
        }

        public CouponServiceResponse<Coupon> UpdateCouponStatus(CouponUpdated ev, List<ServiceLogRecord> logRecords = null)
        {
            // Create the watch
            var sw = new Stopwatch();
            sw.Start();

            // Create a log record collection if necessary
            if (logRecords == null)
            {
                logRecords = new List<ServiceLogRecord>();
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Coupon status update request received."
            });

            // Create a response object
            var response = new CouponServiceResponse<Coupon>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data
            List<string> dataErrors = new List<string>();

            if (ev.CouponID == default(int))
            {
                dataErrors.Add("No valid Coupon id found!");
            }

            if (dataErrors.Count > 0)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = dataErrors.Count + " error(s) found within the posted data! Terminating the process. Errors:" + String.Join(";", dataErrors)
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "There are some errors with the incoming request data!";
                response.Errors.AddRange(dataErrors);
                response.LogRecords = logRecords;

                return response;
            }

            #endregion

            #region [ Data manuplation ]


            #endregion

            // Stop the timer
            sw.Stop();

            // Set the pre-processing time and start the time
            response.PreProcessingTook = sw.ElapsedMilliseconds;
            sw.Start();

            #region [ Load the reseller application ]

            Coupon Coupon = _couponServiceBase.Get(r => r.CouponID == ev.CouponID).Result.FirstOrDefault();

            if (Coupon == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Coupon found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Coupon found with the given id!";
                response.Errors.Add("No Coupon found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Coupon loaded."
            });

            // Update the Coupon status
            Coupon.Status = ev.Status;
            Coupon.Updated = DateTime.UtcNow;


            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.UtcNow,
                Body = "Coupon status updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _couponServiceBase.Update(Coupon);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.UtcNow,
                    Body = "There was an error while updating the Coupon status!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating Coupon!";
                response.Errors.AddRange(baseServiceResponse.Errors);
                response.LogRecords = logRecords;

                return response;
            }
            else
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.UtcNow,
                    Body = string.Format("Coupon status successfuly updated. CouponId:{0}",
                                                Coupon.CouponID)
                });

                // Set the output information
                response.Result.Add(Coupon);
                response.CouponID = Coupon.CouponID;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Coupon status successfuly updated. CouponId:{0}",
                                                Coupon.CouponID);
            response.LogRecords = logRecords;

            return response;
        }
        #endregion
    }
}
