using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Favorites;
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
    public class FavoriteProductService : IFavoriteProductService
    {
        private FavoriteProductServiceBase _favoriteProductServiceBase = new FavoriteProductServiceBase();

        #region [ Queries ]

        public FavoriteProductServiceResponse<FavoriteProducts> GetFavoriteProducts(FavoriteProductRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new FavoriteProductServiceResponse<FavoriteProducts>();

            var FavoriteProducts = new List<FavoriteProducts>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<FavoriteProducts, bool>> filterPredicate = PredicateBuilder.New<FavoriteProducts>(true);

            // Add the filters
            if (req.UserIds.Any())
            {
                filterPredicate = filterPredicate.And(m => req.UserIds.Contains(m.UserId));
            }


            #endregion

            // Make the query
            if (filterPredicate.Parameters.Count > 0)
            {
                FavoriteProducts = _favoriteProductServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
            }
            else
            {
                FavoriteProducts = _favoriteProductServiceBase.GetAllIncluding(includes.ToArray()).Result;
            }

            response.Type = Headstone.Common.ServiceResponseTypes.Success;
            response.Result = FavoriteProducts;

            return response;
        }

        #endregion

        #region [ FavoriteProduct ]

        public FavoriteProductServiceResponse<FavoriteProducts> CreateFavoriteProduct(FavoriteProductCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Create FavoriteProduct request received."
            });

            var response = new FavoriteProductServiceResponse<FavoriteProducts>();

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
            //    dataErrors.Add("No FavoriteProduct item(s)!");
            //}

            //if (String.IsNullOrEmpty(ev.Name))
            //{
            //    dataErrors.Add("No product name!");
            //}
            //if (ev.Items.Count == 0)
            //{
            //    dataErrors.Add("No FavoriteProduct item");
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
                Body = "Creating FavoriteProduct."
            });

            // Create the reseller application
            var command = new FavoriteProducts()
            {
                UserId = ev.UserId,
                ProductId = ev.ProductId,
                Created = DateTime.Now
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("FavoriteProduct created", ev.UserToken, ev.SessionId)
            });
            #endregion

            #region [ Save FavoriteProduct ]

            var baseServiceResponse = _favoriteProductServiceBase.Create(command);
            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving FavoriteProduct!"
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
                    Body = string.Format("FavoriteProduct successfuly created. FavoriteProductId:{0}",
                                            command.FavId)
                });

                // Add the new object to the result
                response.Result.Add(command);

            }
            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("FavoriteProduct successfuly created. FavoriteProductId:{0}",
                                            command.FavId);
            response.LogRecords = logRecords;

            #endregion

            return response;
        }

        public FavoriteProductServiceResponse<FavoriteProducts> DeleteFavoriteProduct(FavoriteProductDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "FavoriteProduct delete request received."
            });

            // Create a response object
            var response = new FavoriteProductServiceResponse<FavoriteProducts>();

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

            if (ev.ProductId == default(int))
            {
                dataErrors.Add("No valid Product id found!");
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


            FavoriteProducts FavoriteProduct = _favoriteProductServiceBase.Get(r => r.ProductId == ev.ProductId).Result.FirstOrDefault();

            if (FavoriteProduct == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No FavoriteProduct found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No FavoriteProduct found with the given id!";
                response.Errors.Add("No FavoriteProduct found with the given id!");
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
                Body = "Deleting FavoriteProduct."
            });

            // Delete the billing info
            var baseServiceResponse = _favoriteProductServiceBase.Delete(FavoriteProduct);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the FavoriteProduct!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the FavoriteProduct!";
                response.Errors.Add("There was an error while deleting the FavoriteProduct!");
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
                    Body = string.Format("FavoriteProduct successfuly deleted. FavoriteProductId:{0}",
                                            FavoriteProduct.FavId)
                });

                // Add the new object to the result
                response.Result.Add(FavoriteProduct);

            }

            #endregion


            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("FavoriteProduct successfuly deleted. FavoriteProductId:{0}",
                                            FavoriteProduct.FavId);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion
    }
}
