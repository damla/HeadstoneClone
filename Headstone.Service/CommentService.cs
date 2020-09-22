
using Headstone.Service.Base;
using Headstone.Service.Interfaces;
using Headstone.Service.Validators.Comment;
using FluentValidation.Results;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Requests;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Headstone.Models.Events.Comment;
using Headstone.Common;

namespace Headstone.Service
{
    public class CommentService : ICommentService
    {
        private CommentServiceBase _commentServiceBase = new CommentServiceBase();
        private CommentPropertyServiceBase _propertyServiceBase = new CommentPropertyServiceBase();
        private CommentTagServiceBase _tagServiceBase = new CommentTagServiceBase();

        #region [ Queries ]

        public CommentServiceResponse<Comment> GetComments(CommentRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new CommentServiceResponse<Comment>();

            var comments = new List<Comment>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();
            includes.Add("Properties");
            includes.Add("Tags");

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<Comment, bool>> filterPredicate = PredicateBuilder.New<Comment>(true);

            // Add the filters
            if (req.CommentIds.Any())
            {
                filterPredicate = filterPredicate.And(r => req.CommentIds.Contains(r.CommentId));
            }

            if (req.CommentTypes.Any())
            {
                filterPredicate = filterPredicate.And(m => req.CommentTypes.Contains(m.Type));
            }

            if (req.RelatedDataEntityIds.Any())
            {
                filterPredicate = filterPredicate.And(m => req.RelatedDataEntityIds.Contains(m.RelatedDataEntityId));
            }

            if (req.RelatedDataEntityTypes.Any())
            {
                filterPredicate = filterPredicate.And(m => req.RelatedDataEntityTypes.Contains(m.RelatedDataEntityType));
            }

            if (req.CommentTags.Any())
            {
                var tagFilter = req.CommentTags.Select(t => t.Type + "|" + t.Value).ToList();

                filterPredicate = filterPredicate.And(d => d.Tags.Any(t => tagFilter.Contains(t.Type + "|" + t.Value)));
            }

            if (req.CommentProperties.Any())
            {
                var propertyFilter = req.CommentProperties.Select(p => p.Key + "|" + p.Value).ToList();

                filterPredicate = filterPredicate.And(d => d.Properties.Select(dp => dp.Key + "|" + dp.Value).Intersect(propertyFilter).Any());
            }

            #endregion

            // Make the query
            if (filterPredicate.Parameters.Count > 0)
            {
                comments = _commentServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
            }
            else
            {
                comments = _commentServiceBase.GetAllIncluding(includes.ToArray()).Result;
            }

            response.Type = Headstone.Common.ServiceResponseTypes.Success;
            response.Result = comments;

            return response;
        }

        #endregion

        #region [ Comment ]

        public CommentServiceResponse<Comment> CreateComment(CommentCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Create comment request received."
            });

            var response = new CommentServiceResponse<Comment>();

            #region [ Validate Request ]

            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming event data."
            });

            // Check required data
            List<string> dataErrors = new List<string>();

            if(ev.UserId == default(int))
            {
                dataErrors.Add("No UserId");
            }

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
                Body = "Creating comment."
            });

            // Create the reseller application
            var command = new Comment()
            {
                ParentId = ev.ParentId,
                UserId = ev.UserId,
                RelatedDataEntityType = ev.RelatedDataEntityType,
                RelatedDataEntityId = ev.RelatedDataEntityId,
                Type = ev.Type,
                Rating = ev.Rating,
                Body = ev.Body,
                Status = ev.Status,
                Created = DateTime.Now,
                Tags= ev.Tags,
                Properties = ev.Properties
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Comment created", ev.UserToken, ev.SessionId)
            });
            #endregion

            #region [ Save Comment ]

            var baseServiceResponse = _commentServiceBase.Create(command);
            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving comment!"
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
                    Body = string.Format("Comment successfuly created. CommentId:{0}",
                                            command.CommentId)
                });

                // Add the new object to the result
                response.Result.Add(command);

                // Set the object id
                response.CommentId = command.CommentId;
            }
            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment successfuly created. CommentId:{0}",
                                            command.CommentId);
            response.LogRecords = logRecords;

            #endregion

            return response;
        }

        public CommentServiceResponse<Comment> DeleteComment(CommentDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment delete request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<Comment>();

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

            if (ev.CommentId == default(int))
            {
                dataErrors.Add("No valid comment id found!");
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

            #region [ Load the object ]

            Comment comment = _commentServiceBase.Get(r => r.CommentId == ev.CommentId).Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No comment found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No comment found with the given id!";
                response.Errors.Add("No comment found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }
            #endregion

            #region [ Delete the object ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Deleting comment."
            });

            // Delete the billing info
            var baseServiceResponse = _commentServiceBase.Delete(comment);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the comment!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the comment!";
                response.Errors.Add("There was an error while deleting the comment!");
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
                    Body = string.Format("Comment successfuly deleted. CommentId:{0}",
                                            comment.CommentId)
                });

                // Add the new object to the result
                response.Result.Add(comment);

                // Set the role id
                response.CommentId = comment.CommentId;
            }

            #endregion


            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment successfuly deleted. CommentId:{0}",
                                            comment.CommentId);
            response.LogRecords = logRecords;

            return response;
        }

        public CommentServiceResponse<Comment> UpdateComment(CommentUpdated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment update request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<Comment>();

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

            if (ev.CommentId == default(int))
            {
                dataErrors.Add("No valid comment id found!");
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

            #region [ Load the object ]

            Comment comment = _commentServiceBase.Get(r => r.CommentId == ev.CommentId).Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No comment found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No comment found with the given id!";
                response.Errors.Add("No comment found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Comment loaded."
            });

            // Update the reseller application
            comment.Body = ev.Body;
            comment.ParentId = ev.ParentId;
            comment.UserId = ev.UserId;
            comment.RelatedDataEntityType = ev.RelatedDataEntityType;
            comment.RelatedDataEntityId = ev.RelatedDataEntityId;
            comment.Type = ev.Type;
            comment.Rating = ev.Rating;
            comment.Status = ev.Status;
            comment.Updated = DateTime.Now;

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Comment updated."
            });

            #endregion

            #region [ Save the object ]

            // Save the address
            var baseServiceResponse = _commentServiceBase.Update(comment);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while updating the comment!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating comment!";
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
                    Body = string.Format("Comment successfuly updated. CommentId:{0}",
                                                comment.CommentId)
                });

                // Set the output information
                response.Result.Add(comment);
                response.CommentId = comment.CommentId;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment successfuly updated. CommentId:{0}",
                                                comment.CommentId);
            response.LogRecords = logRecords;

            return response;
        }

        public CommentServiceResponse<Comment> UpdateCommentStatus(CommentUpdated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment status update request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<Comment>();

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

            if (ev.CommentId == default(int))
            {
                dataErrors.Add("No valid comment id found!");
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

            Comment comment = _commentServiceBase.Get(r => r.CommentId == ev.CommentId).Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No comment found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No comment found with the given id!";
                response.Errors.Add("No comment found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Comment loaded."
            });

            // Update the comment status
            comment.Status = ev.Status;
            comment.Updated = DateTime.UtcNow;


            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.UtcNow,
                Body = "Comment status updated."
            });

            #endregion

            #region [ Save reseller application ]

            // Save the address
            var baseServiceResponse = _commentServiceBase.Update(comment);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.UtcNow,
                    Body = "There was an error while updating the comment status!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while updating comment!";
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
                    Body = string.Format("Comment status successfuly updated. CommentId:{0}",
                                                comment.CommentId)
                });

                // Set the output information
                response.Result.Add(comment);
                response.CommentId = comment.CommentId;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment status successfuly updated. CommentId:{0}",
                                                comment.CommentId);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion

        #region [ Comment Properties ]

        public CommentServiceResponse<CommentProperty> CreateCommentProperty(CommentPropertyCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment property creation request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<CommentProperty>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data

            CommentPropertyCreatedValidator validator = new CommentPropertyCreatedValidator();
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

            #region [ Create comment product ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Creating comment property."
            });

            //CommentProperty commentProperty = ev.ToEntity<CommentProperty>();

            CommentProperty commentProperty = new CommentProperty()
            {
                Key = ev.Key,
                Value = ev.Value,
                Created = DateTime.Now,
                Extra = ev.Extra,
                CommentId = ev.CommentId,
                Status = ev.Status
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Comment property created. UserToken:{0}; SessionId:{1}; ", ev.UserToken, ev.SessionId)
            });

            #endregion

            #region [ Find related comment ]

            Comment comment = _commentServiceBase.Get(c => c.CommentId == ev.CommentId).Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Could not find the related comment to create the property from! Please use a valid comment reference number."
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "Could not find the related comment to create the property from! Please use a valid comment reference number.";
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Related comment loaded."
            });

            #endregion

            #region [ Save comment property ]

            // Save the comment
            var baseServiceResponse = _propertyServiceBase.Create(commentProperty);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while creting the comment property!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.General_Exception.ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while creting the comment property!";
                response.Errors.Add("There was an error while creting the comment property!");
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
                    Body = string.Format("Comment property successfuly created. CommentPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentProperty.PropertyId, ev.UserToken, ev.SessionId)
                });

                // Add the new comment object to the result
                response.Result.Add(commentProperty);

                // Set the comment property id
                response.CommentId = commentProperty.CommentId;
                response.PropertyId = commentProperty.PropertyId;
            }


            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = HeadstoneServiceResponseCodes.Request_Successfuly_Completed.ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment property successfuly created. CommentPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentProperty.PropertyId, ev.UserToken, ev.SessionId);
            response.LogRecords = logRecords;

            return response;
        }

        public CommentServiceResponse<CommentProperty> DeleteCommentProperty(CommentPropertyDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment item delete request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<CommentProperty>();

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
                dataErrors.Add("No valid comment property id found!");
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


            #region [ Find related comment ]

            Comment comment = _commentServiceBase.GetIncluding(c => c.CommentId == ev.CommentId, "Properties").Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Could not find the related comment to remove the property from! Please use a valid comment reference number."
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "Could not find the related comment to remove the property from! Please use a valid comment reference number.";
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Related comment loaded."
            });

            #endregion


            #region [ Load the comment property ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Loading the comment property."
            });

            CommentProperty commentProperty;
            if (ev.PropertyId > 0)
            {
                commentProperty = comment.Properties.FirstOrDefault(p => p.PropertyId == ev.PropertyId);
            }
            else
            {
                commentProperty = comment.Properties.FirstOrDefault(p => p.Key == ev.Key);

            }

            if (commentProperty == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No comment property found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.Invalid_Request.ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No comment property found with the given id!";
                response.Errors.Add("No comment property found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            #endregion

            #region [ Delete comment property ] 

            // Save the comment
            var baseServiceResponse = _propertyServiceBase.Delete(commentProperty);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the comment property!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = HeadstoneServiceResponseCodes.General_Exception.ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the comment property!";
                response.Errors.Add("There was an error while deleting the comment property!");
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
                    Body = string.Format("Comment property successfuly deleted. commentPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentProperty.PropertyId, ev.UserToken, ev.SessionId)
                });

                // Add the new comment object to the result
                response.Result.Add(commentProperty);

                // Set the content entity id
                response.CommentId = commentProperty.CommentId;
                response.PropertyId = commentProperty.PropertyId;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = HeadstoneServiceResponseCodes.Request_Successfuly_Completed.ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment property successfuly deleted. CommentPropertyId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentProperty.PropertyId, ev.UserToken, ev.SessionId);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion

        #region [ Comment Tags ]

        public CommentServiceResponse<CommentTag> CreateCommentTag(CommentTagCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment tag creation request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<CommentTag>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data

            CommentTagCreatedValidator validator = new CommentTagCreatedValidator();
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

            #region [ Create comment tag ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Creating comment tag."
            });


            //CommentTag commentTag = ev.ToEntity<CommentTag>();
            CommentTag commentTag = new CommentTag()
            {
                CommentId = ev.CommentId,
                Created = DateTime.Now,
                Status = ev.Status,
                Type = ev.Type,
                Value = ev.Value
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Comment tag created. UserToken:{0}; SessionId:{1}; ", ev.UserToken, ev.SessionId)
            });

            #endregion

            #region [ Find related comment ]

            Comment comment = _commentServiceBase.Get(c => c.CommentId == ev.CommentId).Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Could not find the related comment to create the tag from! Please use a valid comment reference number."
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "Could not find the related comment to create the tag from! Please use a valid comment reference number.";
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Related comment loaded."
            });

            #endregion

            #region [ Save comment tag ]

            // Save the comment
            var baseServiceResponse = _tagServiceBase.Create(commentTag);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while creting the comment tag!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while creting the comment tag!";
                response.Errors.Add("There was an error while creting the comment tag!");
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
                    Body = string.Format("Comment tag successfuly created. CommentTagId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentTag.TagId, ev.UserToken, ev.SessionId)
                });

                // Add the new comment object to the result
                response.Result.Add(commentTag);

                // Set the comment tag id
                response.CommentId = commentTag.CommentId;
                response.TagId = commentTag.TagId;
            }


            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment tag successfuly created. CommentTagId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentTag.TagId, ev.UserToken, ev.SessionId);
            response.LogRecords = logRecords;

            return response;
        }

        public CommentServiceResponse<CommentTag> DeleteCommentTag(CommentTagDeleted ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Comment tag removal request received."
            });

            // Create a response object
            var response = new CommentServiceResponse<CommentTag>();

            #region [ Validate request ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "User has the required permissions. Now validating the incoming data."
            });

            // Check required data

            CommentTagDeletedValidator validator = new CommentTagDeletedValidator();
            ValidationResult validationResult = validator.Validate(ev);
            if (!validationResult.IsValid)
            {
                List<string> dataErrors = validationResult.Errors.Select(er => er.ErrorMessage).ToList();

                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = dataErrors.Count + " error(s) found within the posted data! Terminating the process. Errors: " + String.Join(";", dataErrors)
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

            #region [ Find related comment ]

            Comment comment = _commentServiceBase.GetIncluding(c => c.CommentId == ev.CommentId, "Tags").Result.FirstOrDefault();

            if (comment == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Could not find the related comment to remove the tag from! Please use a valid comment reference number."
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "Could not find the related comment to remove the tag from! Please use a valid comment reference number.";
                response.LogRecords = logRecords;

                return response;
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Related comment loaded."
            });

            #endregion


            #region [ Load the comment tag ]

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Loading the comment tag."
            });

            CommentTag commentTag;
            if (ev.TagId > 0)
            {
                commentTag = comment.Tags.FirstOrDefault(p => p.TagId == ev.TagId);
            }
            else
            {
                commentTag = comment.Tags.FirstOrDefault(p => p.Type == ev.Type && p.Value == ev.Value);
            }

            if (commentTag == null)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "No comment tag found with the given id!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.Invalid_Request).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "No comment tag found with the given id!";
                response.Errors.Add("No comment tag found with the given id!");
                response.LogRecords = logRecords;

                return response;
            }

            #endregion

            #region [ Delete comment tag ] 

            // Save the comment
            var baseServiceResponse = _tagServiceBase.Delete(commentTag);

            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while deleting the comment tag!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = ServiceResponseTypes.Error;
                response.Code = ((short)HeadstoneServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while deleting the comment tag!";
                response.Errors.Add("There was an error while deleting the comment tag!");
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
                    Body = string.Format("Comment tag successfuly deleted. CommentTagId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentTag.TagId, ev.UserToken, ev.SessionId)
                });

                // Add the new comment object to the result
                response.Result.Add(commentTag);

                // Set the content entity id
                response.CommentId = commentTag.CommentId;
                response.TagId = commentTag.TagId;
            }

            #endregion

            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Comment tag successfuly deleted. CommentTagId:{0}; UserToken:{1}; SessionId:{2}",
                                            commentTag.TagId, ev.UserToken, ev.SessionId);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion
    }
}
