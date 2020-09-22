using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Basket;
using Headstone.Models.Requests;
using System.Collections.Generic;

namespace Headstone.Service.Interfaces
{
    public interface IBasketItemService
    {
        #region [ Queries ]

        BasketItemServiceResponse<BasketItem> GetBasketItems(BasketItemRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ BasketItem ]

        BasketItemServiceResponse<BasketItem> CreateBasketItem(BasketItemCreated ev, List<ServiceLogRecord> logRecords = null);

        BasketItemServiceResponse<BasketItem> UpdateBasketItem(BasketItemUpdated ev, List<ServiceLogRecord> logRecords = null);

        BasketItemServiceResponse<BasketItem> UpdateBasketItemStatus(BasketItemUpdated ev, List<ServiceLogRecord> logRecords = null);

        BasketItemServiceResponse<BasketItem> DeleteBasketItem(BasketItemDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}
