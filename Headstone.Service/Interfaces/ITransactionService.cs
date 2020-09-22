using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Order;
using Headstone.Models.Requests;
using System.Collections.Generic;

namespace Headstone.Service.Interfaces
{
    public interface ITransactionService
    {
        #region [ Queries ]

        TransactionServiceResponse<Transaction> GetTransactions(TransactionRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ Transaction ]

        TransactionServiceResponse<Transaction> CreateTransaction(TransactionCreated ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}