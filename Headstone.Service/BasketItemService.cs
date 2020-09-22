using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Basket;
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
    public class BasketItemService : IBasketItemService
    {
        private BasketItemServiceBase _basketItemServiceBase = new BasketItemServiceBase();

        #region [ Queries ]

        public BasketItemServiceResponse<BasketItem> GetBasketItems(BasketItemRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new BasketItemServiceResponse<BasketItem>();

            var BasketItems = new List<BasketItem>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<BasketItem, bool>> filterPredicate = PredicateBuilder.New<BasketItem>(true);

            // Add the filters
            if (req.BasketItemIDs.Any())
            {
                filterPredicate = filterPredicate.And(r => req.BasketItemIDs.Contains(r.BasketItemID));
            }

            if (req.BasketIDs.Any())
            {
                filterPredicate = filterPredicate.And(m => req.BasketIDs.Contains(m.BasketID));
            }

            if (req.ProductIDs.Any())
            {
                filterPredicate = filterPredicate.And(m => req.ProductIDs.Contains(m.ProductID));
            }

            #endregion

            // Make the query
            if (filterPredicate.Parameters.Count > 0)
            {
                BasketItems = _basketItemServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
            }
            else
            {
                BasketItems = _basketItemServiceBase.GetAllIncluding(includes.ToArray()).Result;
            }

            response.Type = Headstone.Common.ServiceResponseTypes.Success;
            response.Result = BasketItems;

            return response;
        }

        #endregion

        #region [ BasketItem ]

        public BasketItemServiceResponse<BasketItem> CreateBasketItem(BasketItemCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Create BasketItem request received."
            });

            var response = new BasketItemServiceResponse<BasketItem>();

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
            //    dataErrors.Add("No BasketItem item(s)!");
            //}

            //if (String.IsNullOrEmpty(ev.Name))
            //{
            //    dataErrors.Add("No product name!");
            //}
            //if (ev.Items.Count == 0)
            //{
            //    dataErrors.Add("No BasketItem item");
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
                Body = "Creating BasketItem."
            });

            // Create the reseller application
            var command = new BasketItem()
            {
                BasketID = ev.BasketID,
                ProductID = ev.ProductID,
                Quantity = ev.Quantity,
                BasePrice = ev.BasePrice,
                TotalPrice = ev.TotalPrice,
                Status = ev.Status,
                Created = DateTime.Now
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("BasketItem created", ev.UserToken, ev.SessionId)
            });
            #endregion

            #region [ Save BasketItem ]

            var baseServiceResponse = _basketItemServiceBase.Create(command);
            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving BasketItem!"
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
                    Body = string.Format("BasketItem successfuly created. BasketItemId:{0}",
                                            command.BasketItemID)
                });

                // Add the new object to the result
                response.Result.Add(command);

                // Set the object id
                response.BasketItemID = command.BasketItemID;
            }
            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("BasketItem successfuly created. BasketItemId:{0}",
                                            command.BasketItemID);
            response.LogRecords = logRecords;

            #endregion

            return response;
        }

        public BasketItemServiceResponse<BasketItem> DeleteBasketItem(BasketItemDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "BasketItem delete request received."
            });

            // Create a response object
            var response = new BasketItemServiceResponse<BasketItem>();

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

            if (ev.BasketItemID == default(int))
            {
                dataErrors.Add("No valid BasketItem id found!");
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


            BasketItem BasketItem = _basketItemServiceBase.Get(r => r.BasketItemID == ev.BasketItemID).Result.FirstOrDefault();

            if (BasketItem == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No BasketItem found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No BasketItem found with the given id!";
                response.Errors.Add("No BasketItem found with the given id!");
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
                Body = "Deleting BasketItem."
            });

            // Delete the billing info
            var baseServiceResponse = _basketItemServiceBase.Delete(BasketItem);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the BasketItem!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the BasketItem!";
                response.Errors.Add("There was an error while deleting the BasketItem!");
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
                    Body = string.Format("BasketItem successfuly deleted. BasketItemId:{0}",
                                            BasketItem.BasketItemID)
                });

                // Add the new object to the result
                response.Result.Add(BasketItem);

                // Set the role id
                response.BasketItemID = BasketItem.BasketItemID;
            }

            #endregion


            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("BasketItem successfuly deleted. BasketItemId:{0}",
                                            BasketItem.BasketItemID);
            response.LogRecords = logRecords;

            return response;
        }

        public BasketItemServiceResponse<BasketItem> UpdateBasketItem(BasketItemUpdated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "BasketItem update request received."
            });

            // Create a response object
            var response = new BasketItemServiceResponse<BasketItem>();

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

            if (ev.BasketItemID == default(int))
            {
                dataErrors.Add("No valid BasketItem id found!");
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

            BasketItem BasketItem = _basketItemServiceBase.Get(r => r.BasketItemID == ev.BasketItemID).Result.FirstOrDefault();

            if (BasketItem == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No BasketItem found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No BasketItem found with the given id!";
                response.Errors.Add("No BasketItem found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "BasketItem loaded."
            });

            // Update the reseller application
            BasketItem.BasketID = ev.BasketID;
            BasketItem.ProductID = ev.ProductID;
            BasketItem.Quantity = ev.Quantity;
            BasketItem.BasePrice = ev.BasePrice;
            BasketItem.TotalPrice = ev.TotalPrice;
            BasketItem.Status = ev.Status;
            BasketItem.Updated = DateTime.Now;

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "BasketItem updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _basketItemServiceBase.Update(BasketItem);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while updating the BasketItem!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating BasketItem!";
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
                    Body = string.Format("BasketItem successfuly updated. BasketItemId:{0}",
                                                BasketItem.BasketItemID)
                });

                // Set the output information
                response.Result.Add(BasketItem);
                response.BasketItemID = BasketItem.BasketItemID;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("BasketItem successfuly updated. BasketItemId:{0}",
                                                BasketItem.BasketItemID);
            response.LogRecords = logRecords;

            return response;
        }

        public BasketItemServiceResponse<BasketItem> UpdateBasketItemStatus(BasketItemUpdated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "BasketItem status update request received."
            });

            // Create a response object
            var response = new BasketItemServiceResponse<BasketItem>();

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

            if (ev.BasketItemID == default(int))
            {
                dataErrors.Add("No valid BasketItem id found!");
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

            BasketItem BasketItem = _basketItemServiceBase.Get(r => r.BasketItemID == ev.BasketItemID).Result.FirstOrDefault();

            if (BasketItem == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No BasketItem found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No BasketItem found with the given id!";
                response.Errors.Add("No BasketItem found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "BasketItem loaded."
            });

            // Update the BasketItem status
            BasketItem.Status = ev.Status;
            BasketItem.Updated = DateTime.UtcNow;


            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.UtcNow,
                Body = "BasketItem status updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _basketItemServiceBase.Update(BasketItem);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.UtcNow,
                    Body = "There was an error while updating the BasketItem status!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating BasketItem!";
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
                    Body = string.Format("BasketItem status successfuly updated. BasketItemId:{0}",
                                                BasketItem.BasketItemID)
                });

                // Set the output information
                response.Result.Add(BasketItem);
                response.BasketItemID = BasketItem.BasketItemID;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("BasketItem status successfuly updated. BasketItemId:{0}",
                                                BasketItem.BasketItemID);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion

    }
}
