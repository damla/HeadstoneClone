using Headstone.Common.Responses;
using Headstone.MetaData.SDK.Net._452;
using Headstone.Models;
using Headstone.Models.Events.Basket;
using Headstone.Service;
using Headstone.UI.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Headstone.UI.Controllers
{
    public class BasketController : BaseController
    {
        private BasketService basketService = new BasketService();
        private MetaDataServiceClient metaDataServiceClient = new MetaDataServiceClient();
        public CampaingService campaignService = new CampaingService();

        [Route("sepet")]
        public ActionResult Details()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Sepet"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            var model = new BasketViewModel();

            var basket = basketService.GetBaskets(new Headstone.Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();

            var product = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            {
                ProductIds = basket.BasketItems.Select(p => p.ProductID).ToList(),
                Envelope = "full"
            }).Result;



            if (basket != null && product.Any())
            {
                if (!basket.BasketItems.Any())
                {
                    return RedirectToAction("Index", "Home");
                }
                model = new BasketViewModel().From(basket);

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
                //if usercampaign has product

                if (userCampaign != null)
                {
                    if (userCampaign.CampaignProperties.Any())
                    {
                        var userspecificcampaignproductid = userCampaign.CampaignProperties.FirstOrDefault(k => k.Key == "ProductId")?.Value;
                        var userspecificcampaigncategoryid = userCampaign.CampaignProperties.FirstOrDefault(k => k.Key == "CategoryId")?.Value;

                        if (product.Select(p => p.ProductId.ToString()).Contains(userspecificcampaignproductid))
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
                        else if (product.Select(p => p.CategoryProducts.FirstOrDefault().CategoryId.ToString()).Contains(userspecificcampaigncategoryid))
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

                var productPrice = Convert.ToDecimal(product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value);
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

                model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ProductName = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Name;
                model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ProductDescription = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).ShortDescription;
                model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().OldPrice = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value;
                model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ListPrice = productPrice.ToString();
                model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().Price = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "Price").Value;
            }

            return View(model);
        }

        public ActionResult MiniCart()
        {
            var model = new BasketViewModel();

            var basket = basketService.GetBaskets(new Headstone.Models.Requests.BasketRequest()
            {
                UserIds = new List<int> { CurrentUser.Id }
            }).Result.FirstOrDefault();
            if (basket != null)
            {
                if (basket.BasketItems.Count == 0) { return PartialView(model); } // TODO EKIN KONTROL EDERSEN SEVINIRIM
                var product = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
                {
                    ProductIds = basket.BasketItems.Select(p => p.ProductID).ToList(),
                    Envelope = "full"
                }).Result;

                model = new BasketViewModel().From(basket);
                if (basket.BasketItems.Any())
                {
                    model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ProductName = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Name;
                    model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().ListPrice = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "ListPrice").Value;
                    model.BasketItems.Where(b => b.ProductID == product.FirstOrDefault(p => p.ProductId == b.ProductID).ProductId).FirstOrDefault().Price = product.FirstOrDefault(p => p.ProductId == model.BasketItems.FirstOrDefault(b => b.ProductID == p.ProductId).ProductID).Properties.FirstOrDefault(k => k.Key == "Price").Value;
                }
            }

            return PartialView(model);
        }

    }
}