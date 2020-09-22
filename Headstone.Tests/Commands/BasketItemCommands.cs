using System;
using System.Collections.Generic;
using System.Linq;
using Headstone.Common;
using Headstone.Models;
using Headstone.Models.Events.Basket;
using Headstone.Models.Events.Campaign;
using Headstone.Models.Events.Coupon;
using Headstone.Service;
using Headstone.Service.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Headstone.Tests.Commands
{
    [TestClass]
    public class BasketItemCommands
    {
        private BasketService basketService = new BasketService();
        private BasketItemService basketItemService = new BasketItemService();

        


        [TestMethod]
        public void CreateBasketItem()
        {
            var response = basketItemService.CreateBasketItem(new BasketItemCreated()
            {
                BasketID = 28,
                ProductID = 1,
                Quantity = 5,
                BasePrice = 20, 
                TotalPrice = 100
            });

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void UpdateBasketItem()
        {
            var basket = basketService.GetBaskets(new Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { 1 }
            }).Result.LastOrDefault();

            var response = basketService.CreateBasketItem(new BasketItemCreated()
            {
                BasketID = basket.BasketID,
                ProductID = 1,
                Quantity = 5,
                BasePrice = 20,
                TotalPrice = 100
            });

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteBasketItem()
        {
            var response = basketItemService.DeleteBasketItem(new BasketItemDeleted()
            {
                BasketItemID = 121
            });

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);

        }

        [TestMethod]
        public void UpdateBasketItemStatus()
        {
            var response = basketItemService.UpdateBasketItem(new BasketItemUpdated()
            {
                // TODO: 1 UpdateBasketItemStatus  ?? DAMLA

            });

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);

        }

    }
}
