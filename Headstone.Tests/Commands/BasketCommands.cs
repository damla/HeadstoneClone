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
    public class BasketCommands
    {
        // TODO : DONE

        private BasketService basketService = new BasketService();
        private BasketItemService basketItemService = new BasketItemService();

        [TestMethod]
        public void CreateBasket()
        {
            var basketCommand = new BasketCreated()
            {
                UserID = 1,
                BasketItems = new List<BasketItemCreated>()
                {
                    new BasketItemCreated()
                    {
                        BasePrice = 10,
                        ProductID = 1,
                        Quantity = 2,
                        TotalPrice = 10,
                        Created = DateTime.UtcNow,
                        Status = Framework.Models.EntityStatus.Active
                    }
                }
            };

            var response = basketService.CreateBasket(basketCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void CreateBasketItem()
        {
            var basket = basketService.GetBaskets(new Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { 1 }
            }).Result.LastOrDefault();

            var response = basketItemService.CreateBasketItem(new BasketItemCreated()
            {
                BasketID = basket.BasketID,
                ProductID = 1,
                Quantity = 1,
                BasePrice = 10,
                TotalPrice = 10
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

            var response = basketService.UpdateBasketItem(new BasketItemUpdated()
            {
                BasketID = basket.BasketID,
                BasketItemID = basket.BasketItems[0].BasketItemID,
                ProductID = 1,
                Quantity = 3,
                BasePrice = 20,
                TotalPrice = 20
            });

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteBasketItem()
        {
            var basket = basketService.GetBaskets(new Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { 1 }
            }).Result.LastOrDefault();

            var response = basketItemService.DeleteBasketItem(new BasketItemDeleted()
            {
                BasketItemID = basket.BasketItems[0].BasketItemID
            });

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }

        [TestMethod]
        public void DeleteBasket()
        {
            var basketCommand = new BasketDeleted()
            {
                BasketID = 37
            };

            var response = basketService.DeleteBasket(basketCommand);

            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }
    }
}
