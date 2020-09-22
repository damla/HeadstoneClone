using Headstone.Common.Responses;
using Headstone.Common.Services;
using Headstone.Models;
using Headstone.Models.Events.Order;
using Headstone.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Service.Interfaces
{
    public interface IOrderService
    {
        #region [ Queries ]

        OrderServiceResponse<Order> GetOrders(OrderRequest req, List<ServiceLogRecord> logRecords = null);

        OrderServiceResponse<OrderLine> GetOrderLines(OrderLineRequest req, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ Order ]

        OrderServiceResponse<Order> CreateOrder(OrderCreated ev, List<ServiceLogRecord> logRecords = null);

        OrderServiceResponse<Order> DeleteOrder(OrderDeleted ev, List<ServiceLogRecord> logRecords = null);

        #endregion

        #region [ OrderLine ]

        OrderServiceResponse<OrderLine> CreateOrderLine(OrderLineCreated ev, List<ServiceLogRecord> logRecords = null);

        OrderServiceResponse<OrderLine> DeleteOrderLine(OrderLineDeleted ev, List<ServiceLogRecord> logRecords = null);


        #endregion
    }
}
