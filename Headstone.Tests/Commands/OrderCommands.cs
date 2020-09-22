using System;
using System.Collections.Generic;
using Headstone.Models;
using Headstone.Models.Events.Order;
using Headstone.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Headstone.Tests.Commands
{
    [TestClass]
    public class OrderCommands
    {
        private OrderService orderService = new OrderService();

        // TODO : DONE

        [TestMethod]
        public void CreateOrder()
        {
            var orderCommand = new OrderCreated()
            {
                UserID = 1010,
                OrderLines = new List<OrderLine>()
                {
                    new OrderLine
                    {
                        BasePrice = 10,
                        ProductID = 1,
                        Quantity = 1,
                        TotalPrice = 10
                    }
                },
                BasketID = 30,
                CampaignID = 8,
                TotalPrice = 50
            };

            var response = orderService.CreateOrder(orderCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void CreateOrderLine()
        {
            var orderLineCommand = new OrderLineCreated()
            {
                OrderID = 40,
                ProductID = 1,
                Quantity = 1,
                BasePrice = 10,
                TotalPrice = 40
            };
            var response = orderService.CreateOrderLine(orderLineCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteOrderLine()
        {
            var orderLineCommand = new OrderLineDeleted()
            {
                OrderLineID = 35
            };

            var response = orderService.DeleteOrderLine(orderLineCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteOrder()
        {
            var orderCommand = new OrderDeleted()
            {
                OrderId = 39
            };

            var response = orderService.DeleteOrder(orderCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }
    }
}
