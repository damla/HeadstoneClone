using Headstone.Common;
using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Order;
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
    public class TransactionService : ITransactionService
    {
        private TransactionServiceBase _transactionServiceBase = new TransactionServiceBase();

        #region [ Queries ]

        public TransactionServiceResponse<Transaction> GetTransactions(TransactionRequest req, List<ServiceLogRecord> logRecords = null)
        {
            var response = new TransactionServiceResponse<Transaction>();

            var Transactions = new List<Transaction>();

            #region [ Envelope settings ]

            // Create the including fields according to the envelope
            var includes = new List<string>();

            #endregion

            #region [ Filters ]

            // Check for filters
            Expression<Func<Transaction, bool>> filterPredicate = PredicateBuilder.New<Transaction>(true);

            // Add the filters
            if (req.TransactionIds.Any())
            {
                filterPredicate = filterPredicate.And(r => req.TransactionIds.Contains(r.TransactionID));
            }

            if (req.UserIds.Any())
            {
                filterPredicate = filterPredicate.And(m => req.UserIds.Contains(m.UserID));
            }

            if (req.OrderIds.Any())
            {
                filterPredicate = filterPredicate.And(m => req.OrderIds.Contains(m.OrderID));
            }

            #endregion

            // Make the query
            if (filterPredicate.Parameters.Count > 0)
            {
                Transactions = _transactionServiceBase.GetIncluding(filterPredicate, includes.ToArray()).Result;
            }
            else
            {
                Transactions = _transactionServiceBase.GetAllIncluding(includes.ToArray()).Result;
            }

            response.Type = Headstone.Common.ServiceResponseTypes.Success;
            response.Result = Transactions;

            return response;
        }

        #endregion

        #region [ Transaction ]

        public TransactionServiceResponse<Transaction> CreateTransaction(TransactionCreated ev, List<ServiceLogRecord> logRecords = null)
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
                Body = "Create Transaction request received."
            });

            var response = new TransactionServiceResponse<Transaction>();

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
            //    dataErrors.Add("No Transaction item(s)!");
            //}

            //if (String.IsNullOrEmpty(ev.Name))
            //{
            //    dataErrors.Add("No product name!");
            //}
            //if (ev.Items.Count == 0)
            //{
            //    dataErrors.Add("No Transaction item");
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
                Body = "Creating Transaction."
            });

            // Create the reseller application
            var command = new Transaction()
            {
                CardNumber = ev.CardNumber,
                FirstName = ev.FirstName,
                LastName = ev.LastName,
                OrderID = ev.OrderID,
                Status = Framework.Models.EntityStatus.Active,
                TotalPrice = ev.TotalPrice,
                UserID = ev.UserID,
                Created = DateTime.Now
            };

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Transaction created", ev.UserToken, ev.SessionId)
            });
            #endregion

            #region [ Save Transaction ]

            var baseServiceResponse = _transactionServiceBase.Create(command);
            if (baseServiceResponse.Type != Headstone.Framework.Models.ServiceResponseTypes.Success)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving Transaction!"
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
                    Body = string.Format("Transaction successfuly created. TransactionId:{0}",
                                            command.TransactionID)
                });

                // Add the new object to the result
                response.Result.Add(command);

                // Set the object id
                response.TransactionId = command.TransactionID;
            }
            // Stop the sw
            sw.Stop();

            response.Type = ServiceResponseTypes.Success;
            response.Code = ((short)HeadstoneServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Transaction successfuly created. TransactionId:{0}",
                                            command.TransactionID);
            response.LogRecords = logRecords;

            #endregion

            return response;
        }

        #endregion
    }
}
