using FluentValidation.Results;
using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Campaign;
using Headstone.Models.Requests;
using Headstone.Service.Base;
using Headstone.Service.Interfaces;
using Headstone.Service.Validators.Campaign;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Headstone.Service
{
    public class CampaingService : ICampaignService
    {
        private CampaignServiceBase _CampaignServiceBase = new CampaignServiceBase();
        private CampaignPropertyServiceBase _propertyServiceBase = new CampaignPropertyServiceBase();

        #region [ Queries ]

        public CampaignServiceResponse<Campaign> GetCampaigns(CampaignRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new CampaignServiceResponse<Campaign>();

            var campaigns = new List<Campaign>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();
            includes.Add("CampaignProperties");

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<Campaign, bool>> filterPredicate = PredicateBuilder.New<Campaign>(true);

            // Add the filters
            if (req.CampaignIds.Any())
            {
                filterPredicate = filterPredicate.And(r => req.CampaignIds.Contains(r.CampaignID));
            }

            if (req.CampaignTypes.Any())
            {
                filterPredicate = filterPredicate.And(m => req.CampaignTypes.Contains(m.DiscountType));
            }

            if (req.RelatedDataEntityIds.Any())
            {
                filterPredicate = filterPredicate.And(m => req.RelatedDataEntityIds.Contains(m.RelatedDataEntityID));
            }

            if (req.RelatedDataEntityTypes.Any())
            {
                filterPredicate = filterPredicate.And(m => req.RelatedDataEntityTypes.Contains(m.RelatedDataEntityName));
            }

            if (req.CampaignProperties.Any())
            {
                var propertyFilter = req.CampaignProperties.Select(p => p.Key + "|" + p.Value).ToList();

                filterPredicate = filterPredicate.And(d => d.CampaignProperties.Select(dp => dp.Key + "|" + dp.Value).Intersect(propertyFilter).Any());
            }

            #endregion

            // Make the query
            if (filterPredicate.Parameters.Count > 0)
            {
                campaigns = _CampaignServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
            }
            else
            {
                campaigns = _CampaignServiceBase.GetAllIncluding(includes.ToArray()).Result;
            }

            response.Type = Headstone.Common.ServiceResponseTypes.Success;
            response.Result = campaigns;

            return response;
        }

        #endregion

        #region [ Campaign ]

        public CampaignServiceResponse<Campaign> CreateCampaign(CampaignCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Create Campaign request received."
            });

            var response = new CampaignServiceResponse<Campaign>();

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
            //    dataErrors.Add("No Campaign item(s)!");
            //}

            //if (String.IsNullOrEmpty(ev.Name))
            //{
            //    dataErrors.Add("No product name!");
            //}
            //if (ev.Items.Count == 0)
            //{
            //    dataErrors.Add("No Campaign item");
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
                Body = "Creating Campaign."
            });

            // Create the reseller application
            var command = new Campaign()
            {
                RelatedDataEntityName = ev.RelatedDataEntityName,
                RelatedDataEntityID = ev.RelatedDataEntityID,
                Name = ev.Name,
                ShortDescription = ev.ShortDescription,
                LongDescription = ev.LongDescription,
                DiscountType = ev.DiscountType,
                DiscountAmount = ev.DiscountAmount,
                Status = ev.Status,
                Created = DateTime.Now,
                CampaignProperties = ev.CampaignProperties
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Campaign created", ev.UserToken, ev.SessionId)
            });
            #endregion

            #region [ Save Campaign ]

            var baseServiceResponse = _CampaignServiceBase.Create(command);
            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving Campaign!"
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
                    Body = string.Format("Campaign successfuly created. CampaignID:{0}",
                                            command.CampaignID)
                });

                // Add the new object to the result
                response.Result.Add(command);

                // Set the object id
                response.CampaignID = command.CampaignID;
            }
            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Campaign successfuly created. CampaignID:{0}",
                                            command.CampaignID);
            response.LogRecords = logRecords;

            #endregion

            return response;
        }

        public CampaignServiceResponse<Campaign> DeleteCampaign(CampaignDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Campaign delete request received."
            });

            // Create a response object
            var response = new CampaignServiceResponse<Campaign>();

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

            if (ev.CampaignID == default(int))
            {
                dataErrors.Add("No valid Campaign id found!");
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


            Campaign Campaign = _CampaignServiceBase.Get(r => r.CampaignID == ev.CampaignID).Result.FirstOrDefault();

            if (Campaign == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Campaign found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Campaign found with the given id!";
                response.Errors.Add("No Campaign found with the given id!");
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
                Body = "Deleting Campaign."
            });

            // Delete the billing info
            var baseServiceResponse = _CampaignServiceBase.Delete(Campaign);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the Campaign!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the Campaign!";
                response.Errors.Add("There was an error while deleting the Campaign!");
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
                    Body = string.Format("Campaign successfuly deleted. CampaignID:{0}",
                                            Campaign.CampaignID)
                });

                // Add the new object to the result
                response.Result.Add(Campaign);

                // Set the role id
                response.CampaignID = Campaign.CampaignID;
            }

            #endregion


            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Campaign successfuly deleted. CampaignID:{0}",
                                            Campaign.CampaignID);
            response.LogRecords = logRecords;

            return response;
        }

        public CampaignServiceResponse<Campaign> UpdateCampaign(CampaignUpdated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Campaign update request received."
            });

            // Create a response object
            var response = new CampaignServiceResponse<Campaign>();

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

            if (ev.CampaignID == default(int))
            {
                dataErrors.Add("No valid Campaign id found!");
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

            Campaign Campaign = _CampaignServiceBase.Get(r => r.CampaignID == ev.CampaignID).Result.FirstOrDefault();

            if (Campaign == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Campaign found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Campaign found with the given id!";
                response.Errors.Add("No Campaign found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Campaign loaded."
            });

            // Update the reseller application
            Campaign.RelatedDataEntityName = ev.RelatedDataEntityName;
            Campaign.RelatedDataEntityID = ev.RelatedDataEntityID;
            Campaign.Name = ev.Name;
            Campaign.ShortDescription = ev.ShortDescription;
            Campaign.LongDescription = ev.LongDescription;
            Campaign.DiscountType = ev.DiscountType;
            Campaign.DiscountAmount = ev.DiscountAmount;
            Campaign.Status = ev.Status;
            Campaign.Updated = DateTime.Now;

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Campaign updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _CampaignServiceBase.Update(Campaign);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while updating the Campaign!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating Campaign!";
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
                    Body = string.Format("Campaign successfuly updated. CampaignId:{0}",
                                                Campaign.CampaignID)
                });

                // Set the output information
                response.Result.Add(Campaign);
                response.CampaignID = Campaign.CampaignID;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Campaign successfuly updated. CampaignId:{0}",
                                                Campaign.CampaignID);
            response.LogRecords = logRecords;

            return response;
        }

        public CampaignServiceResponse<Campaign> UpdateCampaignStatus(CampaignUpdated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Campaign status update request received."
            });

            // Create a response object
            var response = new CampaignServiceResponse<Campaign>();

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

            if (ev.CampaignID == default(int))
            {
                dataErrors.Add("No valid Campaign id found!");
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

            Campaign Campaign = _CampaignServiceBase.Get(r => r.CampaignID == ev.CampaignID).Result.FirstOrDefault();

            if (Campaign == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Campaign found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Campaign found with the given id!";
                response.Errors.Add("No Campaign found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Campaign loaded."
            });

            // Update the Campaign status
            Campaign.Status = ev.Status;
            Campaign.Updated = DateTime.UtcNow;


            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.UtcNow,
                Body = "Campaign status updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _CampaignServiceBase.Update(Campaign);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.UtcNow,
                    Body = "There was an error while updating the Campaign status!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating Campaign!";
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
                    Body = string.Format("Campaign status successfuly updated. CampaignId:{0}",
                                                Campaign.CampaignID)
                });

                // Set the output information
                response.Result.Add(Campaign);
                response.CampaignID = Campaign.CampaignID;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Campaign status successfuly updated. CampaignId:{0}",
                                                Campaign.CampaignID);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion

        #region [ Campaign Properties ]

        public CampaignServiceResponse<CampaignProperty> CreateCampaignProperty(CampaignPropertyCreated ev, List<ServiceLogRecord> logRecords = null) 
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
                Body = "Campaign property creation request received."
            });

            // Create a response object
            var response = new CampaignServiceResponse<CampaignProperty>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data

            CampaignPropertyCreatedValidator validator = new CampaignPropertyCreatedValidator();
            ValidationResult validationResult = validator.Validate(ev);
            if (!validationResult.IsValid)
            {
                List<string> dataErrors = validationResult.Errors.Select(er => er.ErrorMessage).ToList();
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
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
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

            #region [ Create Campaign product ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Creating Campaign property."
            });

            //CampaignProperty CampaignProperty = ev.ToEntity<CampaignProperty>();

            CampaignProperty CampaignProperty = new CampaignProperty()
            {
                Key = ev.Key,
                Value = ev.Value,
                Created = DateTime.Now,
                Extra = ev.Extra,
                CampaignID = ev.CampaignID,
                Status = ev.Status
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Campaign property created. UserToken:{0}; SessionId:{1}; ", ev.UserToken, ev.SessionId)
            });

            #endregion

            #region [ Find related Campaign ]

            Campaign Campaign = _CampaignServiceBase.Get(c => c.CampaignID == ev.CampaignID).Result.FirstOrDefault();

            if (Campaign == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Could not find the related Campaign to create the property from! Please use a valid Campaign reference number."
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "Could not find the related Campaign to create the property from! Please use a valid Campaign reference number.";
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Related Campaign loaded."
            });

            #endregion

            #region [ Save Campaign property ]

            // Save the Campaign
            var baseServiceResponse = _propertyServiceBase.Create(CampaignProperty);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while creting the Campaign property!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.General_Exception.ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while creting the Campaign property!";
                response.Errors.Add("There was an error while creting the Campaign property!");
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
                    Body = string.Format("Campaign property successfuly created. CampaignPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            CampaignProperty.PropertyId, ev.UserToken, ev.SessionId)
                });

                // Add the new Campaign object to the result
                response.Result.Add(CampaignProperty);

                // Set the Campaign property id
                response.CampaignID = CampaignProperty.CampaignID;
                response.PropertyId = CampaignProperty.PropertyId;
            }


            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = HeadstoneServiceResponseCodes.Request_Successfuly_Completed.ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Campaign property successfuly created. CampaignPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            CampaignProperty.PropertyId, ev.UserToken, ev.SessionId);
            response.LogRecords = logRecords;

            return response;
        }

        public CampaignServiceResponse<CampaignProperty> DeleteCampaignProperty(CampaignPropertyDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Campaign item delete request received."
            });

            // Create a response object
            var response = new CampaignServiceResponse<CampaignProperty>();

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

            if (ev.PropertyId == default(int))
            {
                dataErrors.Add("No valid Campaign property id found!");
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
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
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


            #region [ Find related Campaign ]

            Campaign Campaign = _CampaignServiceBase.GetIncluding(c => c.CampaignID == ev.CampaignID, "CampaignProperties").Result.FirstOrDefault();

            if (Campaign == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Could not find the related Campaign to remove the property from! Please use a valid Campaign reference number."
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "Could not find the related Campaign to remove the property from! Please use a valid Campaign reference number.";
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Related Campaign loaded."
            });

            #endregion


            #region [ Load the Campaign property ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Loading the Campaign property."
            });

            CampaignProperty CampaignProperty;
            if (ev.PropertyId > 0)
            {
                CampaignProperty = Campaign.CampaignProperties.FirstOrDefault(p => p.PropertyId == ev.PropertyId);
            }
            else
            {
                CampaignProperty = Campaign.CampaignProperties.FirstOrDefault(p => p.Key == ev.Key);

            }

            if (CampaignProperty == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No Campaign property found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No Campaign property found with the given id!";
                response.Errors.Add("No Campaign property found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            #endregion

            #region [ Delete Campaign property ] 

            // Save the Campaign
            var baseServiceResponse = _propertyServiceBase.Delete(CampaignProperty);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the Campaign property!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.General_Exception.ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the Campaign property!";
                response.Errors.Add("There was an error while deleting the Campaign property!");
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
                    Body = string.Format("Campaign property successfuly deleted. CampaignPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            CampaignProperty.PropertyId, ev.UserToken, ev.SessionId)
                });

                // Add the new Campaign object to the result
                response.Result.Add(CampaignProperty);

                // Set the content entity id
                response.CampaignID = CampaignProperty.CampaignID;
                response.PropertyId = CampaignProperty.PropertyId;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = HeadstoneServiceResponseCodes.Request_Successfuly_Completed.ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Campaign property successfuly deleted. CampaignPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            CampaignProperty.PropertyId, ev.UserToken, ev.SessionId);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion
    }
}
