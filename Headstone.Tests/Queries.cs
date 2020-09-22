using System;
using System.Data.Entity.Infrastructure;
using Headstone.Common;
using Headstone.Service;
using Headstone.Service.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Headstone.Tests
{
    [TestClass]
    public class Queries
    {
        private GeoLocationService geoLocationService = new GeoLocationService();
        private CouponService couponService = new CouponService();
        private CampaingService campaingService = new CampaingService();
        private CommentService commentService = new CommentService();
        private BasketService basketService = new BasketService();
        private BasketItemService basketItemService = new BasketItemService();
        private TaxOfficeService taxOfficeService = new TaxOfficeService();
        private OrderService orderService = new OrderService();
        private TransactionService transactionService = new TransactionService();

        [TestMethod]
        public void GetGeolocations()
        {
            var geoLocations = geoLocationService.GetGeoLocations(new Models.Requests.GeoLocationRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, geoLocations.Type);
        }

        [TestMethod]
        public void GetCoupons()
        {
            var coupons = couponService.GetCoupons(new Models.Requests.CouponRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, coupons.Type);

        }

        [TestMethod]
        public void GetBaskets()
        {
            var baskets = basketService.GetBaskets(new Models.Requests.BasketRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, baskets.Type);
        }
        [TestMethod]
        public void GetBasketItems()
        {
            var basketItems = basketItemService.GetBasketItems(new Models.Requests.BasketItemRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, basketItems.Type);

        }

        [TestMethod]
        public void GetCampaings()
        {
            var campaigns = campaingService.GetCampaigns(new Models.Requests.CampaignRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, campaigns.Type);

        }

        [TestMethod]
        public void GetComments()
        {
            var comments = commentService.GetComments(new Models.Requests.CommentRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, comments.Type);

        }

        [TestMethod]
        public void TaxOffices()
        {
            var taxOffices = taxOfficeService.GetOffices(new Models.Requests.TaxOfficeRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, taxOffices.Type);
        }

        [TestMethod]
        public void GetOrders()
        {
            var orders = orderService.GetOrders(new Models.Requests.OrderRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, orders.Type);
        }

        [TestMethod]
        public void GetOrderLines()
        {
            var orders = orderService.GetOrderLines(new Models.Requests.OrderLineRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, orders.Type);
        }

        [TestMethod]
        public void GetTransactions()
        {
            var transactions = transactionService.GetTransactions(new Models.Requests.TransactionRequest());

            Assert.AreEqual(ServiceResponseTypes.Success, transactions.Type);
        }
    }
}
