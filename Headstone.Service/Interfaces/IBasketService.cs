using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Basket;
using Headstone.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Interfaces
{
    public interface IBasketService
    {
        #region [ Queries ]

        BasketServiceResponse<Basket> GetBaskets(BasketRequest req, List<ServiceLogRecord> logRecords = null);

        BasketItemServiceResponse<BasketItem> GetBasketItems(BasketItemRequest req, List<ServiceLogRecord> logRecords = null);


        #endregion

        #region [ Basket ]

        BasketServiceResponse<Basket> CreateBasket(BasketCreated ev, List<ServiceLogRecord> logRecords = null);

        BasketServiceResponse<Basket> DeleteBasket(BasketDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ Basket Item ]

        BasketItemServiceResponse<BasketItem> CreateBasketItem(BasketItemCreated ev, List<ServiceLogRecord> logRecords = null);

        BasketItemServiceResponse<BasketItem> UpdateBasketItem(BasketItemUpdated ev, List<ServiceLogRecord> logRecords = null);

        BasketItemServiceResponse<BasketItem> DeleteBasketItem(BasketItemDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion
    }
}
