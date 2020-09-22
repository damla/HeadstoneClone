using Headstone.Models;
using Headstone.Service;
using Headstone.Service.Extensions;
using Headstone.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class OrderController : BaseController
    {
        private OrderService orderService = new OrderService();
        private TransactionService transactionService = new TransactionService();
        private BasketService basketService = new BasketService();
        private Headstone.MetaData.SDK.Net._452.MetaDataServiceClient metaDataServiceClient = new MetaData.SDK.Net._452.MetaDataServiceClient();
        public CampaingService campaignService = new CampaingService();


        [Route("odeme")]
        public ActionResult Index()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Ödeme"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            if (CurrentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var basket = basketService.GetBaskets(new Headstone.Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();

            var product = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            {
                ProductIds = basket.BasketItems.Select(p => p.ProductID).ToList(),
                Envelope = "full"
            }).Result;

            var model = new CheckoutViewModel()
            {
                CurrentUser = CurrentUser,
                Order = new OrderViewModel
                {
                    CurrentUser = CurrentUser,
                    Basket = new BasketViewModel().From(basket),
                    Addresses = identityServiceClient.GetUserAddresses(new Lidia.Identity.API.Models.Queries.UserAddressQuery()
                    {
                        RelatedUserIds = new List<int> { CurrentUser.Id }
                    }).Result,
                    BillingInfos = identityServiceClient.GetUserBillingInfo(new Lidia.Identity.API.Models.Queries.UserBillingInfoQuery()
                    {
                        RelatedUserIds = new List<int> { CurrentUser.Id }
                    }).Result
                },
            };

            //get campaigns if any
            var userCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
            {
                RelatedDataEntityTypes = new List<string> { "User" },
                RelatedDataEntityIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();

            var productCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
            {
                RelatedDataEntityTypes = new List<string> { "Product" },
                RelatedDataEntityIds = product.Select(c => c.ProductId).ToList()
            }).Result.FirstOrDefault();

            var categoryCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
            {
                RelatedDataEntityTypes = new List<string> { "Category" },
                RelatedDataEntityIds = product.SelectMany(c => c.CategoryProducts.Select(k => k.CategoryId)).ToList()
            }).Result.FirstOrDefault();

            #region [ Process Discounts ]

            decimal discountAmount = 0;
            decimal discountPercentage = 1;
            bool isDiscountPercentage = false;
            if (userCampaign != null)
            {
                if (userCampaign.DiscountType == Common.DiscountType.Percentage)
                {
                    isDiscountPercentage = true;
                    discountPercentage = discountPercentage * userCampaign.DiscountAmount / 100;
                }
                else
                {
                    isDiscountPercentage = false;
                    discountAmount += userCampaign.DiscountAmount;
                }
            }
            if (productCampaign != null)
            {
                if (productCampaign.DiscountType == Common.DiscountType.Percentage)
                {
                    isDiscountPercentage = true;
                    discountPercentage = discountPercentage + discountPercentage * userCampaign.DiscountAmount / 100;
                    discountPercentage = discountPercentage > 1 ? discountPercentage - 1 : discountPercentage;
                }
                else
                {
                    isDiscountPercentage = false;
                    discountAmount += userCampaign.DiscountAmount;
                }
            }
            if (categoryCampaign != null)
            {
                if (categoryCampaign.DiscountType == Common.DiscountType.Percentage)
                {
                    isDiscountPercentage = true;
                    discountPercentage = discountPercentage + discountPercentage * userCampaign.DiscountAmount / 100;
                    discountPercentage = discountPercentage > 1 ? discountPercentage - 1 : discountPercentage;
                }
                else
                {
                    isDiscountPercentage = false;
                    discountAmount += userCampaign.DiscountAmount;
                }
            }

            #endregion

            var productPrice = Convert.ToDecimal(product.FirstOrDefault(p => p.ProductId == model.Order.Basket.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value);
            //apply discounts on basket
            if (userCampaign != null || productCampaign != null || categoryCampaign != null)
            {
                if (discountAmount != 0)
                {
                    productPrice = productPrice - discountAmount;
                }
                if (discountPercentage != 1)
                {
                    productPrice = productPrice * discountPercentage;
                }
            }

            model.Order.Basket.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ProductName = product.FirstOrDefault(p => p.ProductId == model.Order.Basket.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Name;
            model.Order.Basket.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ProductDescription = product.FirstOrDefault(p => p.ProductId == model.Order.Basket.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).ShortDescription;
            model.Order.Basket.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().OldPrice = product.FirstOrDefault(p => p.ProductId == model.Order.Basket.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value;
            model.Order.Basket.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ListPrice = productPrice.ToString();
            model.Order.Basket.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().Price = product.FirstOrDefault(p => p.ProductId == model.Order.Basket.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "Price").Value;

            return View(model);
        }

        [HttpPost, Route("odeme/al")]
        public JsonResult Checkout(CheckoutViewModel model)
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Sipariş Özeti"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            //try to get the address first
            var userAddress = identityServiceClient.GetUserAddresses(new Lidia.Identity.API.Models.Queries.UserAddressQuery()
            {
                AddressIds = new List<int> { model.AddressId }
            }).Result.FirstOrDefault();

            //if user address doesnt exist create one
            var address = new Lidia.Identity.API.Models.UserAddress();
            if (userAddress == null)
            {
                var addressCommand = identityServiceClient.CreateUserAddress(new Lidia.Identity.API.Models.Commands.UserAddressCommand()
                {
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    Country = model.Country,
                    Name = model.AddressName,
                    ZipCode = model.ZipCode,
                    UserId = CurrentUser.Id,
                    District = model.District,
                    RelatedUserId = CurrentUser.Id,
                    SessionId = "x",
                    UserToken = "x",
                    Environment = "x"
                });
                address.AddressId = addressCommand.AddressId.Value;
            }
            else
            {
                address.AddressId = model.AddressId;
            }
            //create billing info
            var billinginfoCommand = identityServiceClient.CreateUserBillingInfo(new Lidia.Identity.API.Models.Commands.UserBillingInfoCommand()
            {
                StreetAddress = model.StreetAddress,
                City = model.City,
                Country = model.Country,
                District = model.District,
                Name = model.AddressName,
                UserId = CurrentUser.Id,
                TaxOffice = "",
                TaxNumber = "",
                RelatedUserId = CurrentUser.Id,
                SessionId = "x",
                UserToken = "x",
                Environment = "x"
            });
            var billinginfo = new Lidia.Identity.API.Models.UserBillingInfo()
            {
                BillingInfoId = billinginfoCommand.Result.FirstOrDefault().BillingInfoId
            };
            //get basket

            var basket = basketService.GetBaskets(new Headstone.Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();

            var basketVM = new BasketViewModel().From(basket);

            var product = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            {
                ProductIds = basket.BasketItems.Select(p => p.ProductID).ToList()
            }).Result;

            var userCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
            {
                RelatedDataEntityTypes = new List<string> { "User" },
                RelatedDataEntityIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();

            var productCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
            {
                RelatedDataEntityTypes = new List<string> { "Product" },
                RelatedDataEntityIds = product.Select(c => c.ProductId).ToList()
            }).Result.FirstOrDefault();

            var categoryCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
            {
                RelatedDataEntityTypes = new List<string> { "Category" },
                RelatedDataEntityIds = product.SelectMany(c => c.CategoryProducts.Select(k => k.CategoryId)).ToList()
            }).Result.FirstOrDefault();

            #region [ Process Discounts ]

            decimal discountAmount = 0;
            decimal discountPercentage = 1;
            bool isDiscountPercentage = false;
            if (userCampaign != null)
            {
                if (userCampaign.DiscountType == Common.DiscountType.Percentage)
                {
                    isDiscountPercentage = true;
                    discountPercentage = discountPercentage * userCampaign.DiscountAmount / 100;
                }
                else
                {
                    isDiscountPercentage = false;
                    discountAmount += userCampaign.DiscountAmount;
                }
            }
            if (productCampaign != null)
            {
                if (productCampaign.DiscountType == Common.DiscountType.Percentage)
                {
                    isDiscountPercentage = true;
                    discountPercentage = discountPercentage + discountPercentage * userCampaign.DiscountAmount / 100;
                    discountPercentage = discountPercentage > 1 ? discountPercentage - 1 : discountPercentage;
                }
                else
                {
                    isDiscountPercentage = false;
                    discountAmount += userCampaign.DiscountAmount;
                }
            }
            if (categoryCampaign != null)
            {
                if (categoryCampaign.DiscountType == Common.DiscountType.Percentage)
                {
                    isDiscountPercentage = true;
                    discountPercentage = discountPercentage + discountPercentage * userCampaign.DiscountAmount / 100;
                    discountPercentage = discountPercentage > 1 ? discountPercentage - 1 : discountPercentage;
                }
                else
                {
                    isDiscountPercentage = false;
                    discountAmount += userCampaign.DiscountAmount;
                }
            }

            #endregion

            var productPrice = Convert.ToDecimal(product.FirstOrDefault(p => p.ProductId == basketVM.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value);
            //apply discounts on basket
            if (userCampaign != null || productCampaign != null || categoryCampaign != null)
            {
                if (discountAmount != 0)
                {
                    productPrice = productPrice - discountAmount;
                }
                if (discountPercentage != 1)
                {
                    productPrice = productPrice * discountPercentage;
                }
            }

            basketVM.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().OldPrice = product.FirstOrDefault(p => p.ProductId == basketVM.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value;
            basketVM.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ListPrice = productPrice.ToString();
            basketVM.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().Price = product.FirstOrDefault(p => p.ProductId == basketVM.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "Price").Value;

            //create order first
            var orderCommand = orderService.CreateOrder(new Headstone.Models.Events.Order.OrderCreated()
            {
                AddressID = address.AddressId,
                BasketID = basketVM.BasketId,
                BillingInfoID = billinginfo.BillingInfoId,
                TotalPrice = Convert.ToDecimal(basketVM.TotalListPrice),
                UserID = CurrentUser.Id,
                //CampaingId
                OrderLines = basket.BasketItems.Select(b => new OrderLine()
                {
                    ProductID = b.ProductID,
                    Quantity = b.Quantity,
                    BasePrice = b.BasePrice,
                    TotalPrice = basketVM.TotalListPrice,
                    Status = Framework.Models.EntityStatus.Active,
                    Created = DateTime.Now,
                }).ToList(),
                CampaignID = 8
            });

            var order = orderService.GetOrders(new Headstone.Models.Requests.OrderRequest()
            {
                OrderIds = new List<int> { orderCommand.OrderId },
                Envelope = "full"
            }).Result.FirstOrDefault();

            model.Order = new OrderViewModel().From(order);
            model.Order.Basket = basketVM;
            model.Address = address;
            model.BillingInfo = billinginfo;

            //create transaction
            var transactionCommand = transactionService.CreateTransaction(new Headstone.Models.Events.Order.TransactionCreated()
            {
                CardNumber = model.CardNumber.MaskCreditCard(),
                OrderID = orderCommand.OrderId,
                TotalPrice = orderCommand.Result.FirstOrDefault().TotalPrice,
                FirstName = model.CardHolderName.Split(' ')[0],
                LastName = model.CardHolderName.Split(' ')[1],
                Status = Framework.Models.EntityStatus.Active,
                TimeStamp = DateTime.Now,
                UserID = CurrentUser.Id
            });
            TempData["model"] = model;

            //remove basketitems 
            foreach (var item in basket.BasketItems)
            {
                var removebasketItems = basketService.DeleteBasketItem(new Headstone.Models.Events.Basket.BasketItemDeleted()
                {
                    BasketItemID = item.BasketItemID
                });
            }
            

            return Json("");
        }

        [HttpGet, Route("siparis/ozet")]
        public ActionResult CheckoutGet()
        {
            var model = (CheckoutViewModel)TempData["model"];
            return View("~/Views/Order/Checkout.cshtml", model);
        }

    }
}