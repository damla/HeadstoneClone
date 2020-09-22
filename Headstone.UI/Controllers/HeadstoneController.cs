using Headstone.Common.Responses;
using Headstone.MetaData.API.Models.Live;
using Headstone.MetaData.SDK.Net._452;
using Headstone.Models.Events.Basket;
using Headstone.Service;
using Headstone.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Headstone.UI.Controllers
{
    public class HeadstoneController : BaseController
    {
        private BasketService basketService = new BasketService();
        private MetaDataServiceClient metaDataServiceClient = new MetaDataServiceClient();

        public HeadstoneController Init(RequestContext requestContext)
        {
            this.Initialize(requestContext);

            return this;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        [Route("basket/addtobasket")]
        public JsonResult AddToBasket(int productId, int quantity)
        {
            //check if user has existing basket
            var basket = basketService.GetBaskets(new Headstone.Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();

            var product = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            {
                ProductIds = new List<int> { productId },
                Envelope = "full"
            }).Result.FirstOrDefault();

            if (basket != null)
            {
                if (basket.BasketItems.Select(p => p.ProductID).Contains(productId))
                {
                    //if product is already in the basket

                    //first remove the basketitem
                    var basketItemDeleted = new BasketItemDeleted()
                    {
                        BasketItemID = basket.BasketItems.Where(p => p.ProductID == productId).FirstOrDefault().BasketItemID
                    };

                    var deletebasketitem = basketService.DeleteBasketItem(basketItemDeleted);

                    //then recreate the basketitem with updated quantity
                    var basketItemCreated = new BasketItemCreated()
                    {
                        BasketID = basket.BasketID,
                        ProductID = productId,
                        Quantity = quantity + basket.BasketItems.Where(p => p.ProductID == productId).FirstOrDefault().Quantity,
                    };

                    var basketItemresponse = basketService.CreateBasketItem(basketItemCreated);

                }
                else
                {
                    var basketItemCommand = new BasketItemCreated()
                    {
                        BasketID = basket.BasketID,
                        ProductID = productId,
                        Quantity = quantity,
                    };

                    var bresponse = basketService.CreateBasketItem(basketItemCommand);
                }
            }
            else
            {
                //create basket if it does not exist
                var command = new BasketCreated()
                {
                    UserID = CurrentUser.Id,
                    BasketItems = new List<BasketItemCreated>()
                {
                    new BasketItemCreated()
                    {
                        ProductID = productId,
                        Quantity = quantity,
                        Created = DateTime.Now
                    }
                }
                };

                var response = basketService.CreateBasket(command);

            }

            var model = new BasketViewModel().From(basket);

            model.BasketItems.Where(b => b.ProductID == productId).FirstOrDefault().ProductName = product.Name; // TODO: BURASI HATA VERIYOR!!
            model.BasketItems.Where(b => b.ProductID == productId).FirstOrDefault().ListPrice = product.Properties.FirstOrDefault(k => k.Key == "ListPrice").Value;
            model.BasketItems.Where(b => b.ProductID == productId).FirstOrDefault().Price = product.Properties.FirstOrDefault(k => k.Key == "Price").Value;


            var html = ViewToString("Basket", "~/Views/Basket/Minicart.cshtml", model);

            var controllerResponse = new HeadstoneControllerResponse()
            {
                Html = html
            };

            return Json(controllerResponse, JsonRequestBehavior.AllowGet);
        }

        [Route("basket/changelinequantity")]
        public JsonResult ChangeLineQuantity(int BasketItemID, int quantity)
        {
            //check if user has existing basket
            var basket = basketService.GetBasketItems(new Headstone.Models.Requests.BasketItemRequest()
            {
                BasketItemIDs = new List<int> { BasketItemID }
            }).Result.FirstOrDefault();

            //first remove the basketitem
            var basketItemDeleted = new BasketItemDeleted()
            {
                BasketItemID = BasketItemID
            };

            var deletebasketitem = basketService.DeleteBasketItem(basketItemDeleted);

            //then recreate the basketitem with updated quantity
            var basketItemCreated = new BasketItemCreated()
            {
                BasketID = basket.BasketID,
                ProductID = basket.ProductID,
                Quantity = quantity + basket.Quantity,
            };

            var basketItemresponse = basketService.CreateBasketItem(basketItemCreated);

            var controllerResponse = new HeadstoneControllerResponse()
            {
                Data = new
                {
                    LineId = basketItemresponse.BasketItemID
                },
            };

            return Json(controllerResponse, JsonRequestBehavior.AllowGet);
        }

        [Route("basket/removeline")]
        public JsonResult RemoveLine(int BasketItemID)
        {
            var basketItemDeleted = new BasketItemDeleted()
            {
                BasketItemID = BasketItemID
            };

            var deletebasketitem = basketService.DeleteBasketItem(basketItemDeleted);

            var controllerResponse = new HeadstoneControllerResponse()
            {
                Data = new
                {
                    LineId = deletebasketitem.BasketItemID
                },
            };

            return Json(controllerResponse, JsonRequestBehavior.AllowGet);
        }

        [Route("addtofav")]
        public JsonResult AddToFav(int productId)
        {
            var favItemAdded = identityServiceClient.AddProperty(new Lidia.Identity.API.Models.Commands.UserPropertyCommand()
            {
                RelatedUserId = CurrentUser.Id,
                UserId = CurrentUser.Id,
                Environment = "x",
                SessionId = "x",
                UserToken = "x",
                Key = "FavProduct",
                Value = productId.ToString()
            });

            return Json(productId);
        }

    }
}