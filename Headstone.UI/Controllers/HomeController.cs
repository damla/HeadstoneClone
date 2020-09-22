using Headstone.MetaData.SDK.Net._452;
using Headstone.Service;
using Headstone.UI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class HomeController : BaseController
    {
        private CampaingService campaignService = new CampaingService();
        private MetaDataServiceClient metaDataServiceClient = new MetaDataServiceClient();
        public ActionResult Index()
        {
            var model = new HomeViewModel()
            {
                CurrentUser = CurrentUser,
                RecommendedProducts = new ProductListViewModel()
            };

            if (CurrentUser != null)
            {
                //get user based discounts
                var userCampaign = campaignService.GetCampaigns(new Headstone.Models.Requests.CampaignRequest()
                {
                    RelatedDataEntityTypes = new List<string> { "User" },
                    RelatedDataEntityIds = new List<int> { CurrentUser.Id }
                }).Result.FirstOrDefault();

                if(userCampaign != null)
                {
                    var campaignProductId = userCampaign.CampaignProperties.FirstOrDefault(k => k.Key == "ProductId").Value;
                    var campaignDiscounttype = userCampaign.DiscountType;
                    var discountAmout = userCampaign.DiscountAmount;

                    var product = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
                    {
                        ProductIds = new List<int> { Convert.ToInt32(campaignProductId) },
                        Envelope = "full"
                    }).Result.FirstOrDefault();

                    model.Campaign = new CampaignViewModel()
                    {
                        DiscountAmount = (int)discountAmout,
                        DiscountType = campaignDiscounttype,
                        Product = new ProductViewModel().From(product)
                    };
                }

                //get user
                var user = identityServiceClient.GetUsers(new Lidia.Identity.API.Models.Queries.UserQuery()
                {
                    RelatedUserIds = new List<int> { CurrentUser.Id },
                    Envelope = "full"
                }).Result.FirstOrDefault();

                //get recommended productIds 
                var recommendedProductIds = user.Properties.Where(k => k.Key == "RecommendedProduct").ToList().Select(v => Convert.ToInt32(v.Value)).ToList();

                if (recommendedProductIds.Any())
                {
                    //get recommended products
                    var recommendedProducts = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
                    {
                        ProductIds = recommendedProductIds,
                        Envelope = "full"
                    }).Result;

                    if (recommendedProducts.Any())
                    {
                        model.RecommendedProducts.Products = recommendedProducts.Select(p => new ProductViewModel().From(p)).ToList();
                    }
                }
            }

            return View(model);
        }
    }
}