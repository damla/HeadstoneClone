using Headstone.MetaData.SDK.Net._452;
using Headstone.Models;
using Headstone.Service;
using Headstone.UI.Models.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.UI.Controllers
{
    public class AccountController : BaseController
    {
        private OrderService orderService = new OrderService();
        private CouponService couponService = new CouponService();
        private FavoriteProductService favoriteService = new FavoriteProductService();
        private MetaDataServiceClient metaDataServiceClient = new MetaDataServiceClient();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [Route("siparislerim")]
        public ActionResult OrderInfo()
        {
            var model = new OrderPageViewModel();
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Siparişlerim"
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            var orders = orderService.GetOrders(new Headstone.Models.Requests.OrderRequest()
            {
                UserIds = new List<int> { CurrentUser.Id }
            }).Result;

            var products = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            {
                ProductIds = orders.SelectMany(l => l.OrderLines.Select(p => p.ProductID)).ToList()
            }).Result;

            foreach (var order in orders)
            {
                model.ItemsOrder.Add(new OrderViewModel()
                {
                    
                    OrderID = order.OrderID,
                    TotalPrice = order.TotalPrice,
                    Lines = order.OrderLines,
                });
            }
            foreach (var product in products)
            {
                model.ItemsProducts.Add(new ProductViewModel()
                {
                    Name = product.Name,
                    ShortDescription = product.ShortDescription,
                });
            }
            return View(model);
        }

        [Route("kullanici-bilgilerim")]
        public ActionResult UserInfo()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Kullanıcı Bilgilerim" // degisecek
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            var user = identityServiceClient.GetUsers(new Lidia.Identity.API.Models.Queries.UserQuery()
            {
                RelatedUserIds = new List<int> { CurrentUser.Id },
            }).Result.FirstOrDefault();

            var model = new UserViewModel()
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            };

            return View(model);
        }

        [HttpPost, Route("kullanici-bilgilerim")]
        public ActionResult UserInfo(UserViewModel model)
        {

            return View();
        }

        [Route("favori-urunlerim")]
        public ActionResult Favorites()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Favori Ürünlerim" // degisecek
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            //var favProductIds = favoriteService.GetFavoriteProducts(new Headstone.Models.Requests.FavoriteProductRequest()
            //{
            //    UserIds = new List<int> { CurrentUser.Id }
            //}).Result.Select(p => p.ProductId).ToList();

            //if (favProductIds.Any())
            //{
            //    var products = metaDataServiceClient.GetProducts(new MetaData.API.Models.Queries.Live.ProductQuery()
            //    {
            //        ProductIds = favProductIds,
            //        Envelope = "full"
            //    }).Result;

            //    //add products to model
            //}

            return View();
        }

        [Route("indirim-kuponlarim")]
        public ActionResult Coupons()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "İndirim Kuponlarım" // degisecek
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            var coupons = couponService.GetCoupons(new Headstone.Models.Requests.CouponRequest()
            {
                OwnerIDs = new List<int?> { CurrentUser.Id },
            }).Result;

            var items = new List<Coupon>();

            for (int i = 0; i < coupons.Count; i++)
            {
                var model = new Coupon()
                {
                    CouponCode = coupons[i].CouponCode,
                    ToDate = coupons[i].ToDate,
                    FromDate = coupons[i].FromDate,
                };
                items.Add(model);
            }


            return View(items);
        }

        [Route("adreslerim")]
        public ActionResult Address()
        {
            #region [ Breadcrumb ]

            // Create the breadcrumb
            var breadcrumb = new List<BreadcrumbItemViewModel>();
            breadcrumb.Add(new BreadcrumbItemViewModel()
            {
                Text = "Adreslerim" // degisecek
            });

            ViewBag.Breadcrumb = breadcrumb;

            #endregion

            var user = identityServiceClient.GetUserAddresses(new Lidia.Identity.API.Models.Queries.UserAddressQuery()
            {
                RelatedUserIds = new List<int> { CurrentUser.Id },
            }).Result.FirstOrDefault();

            var model = new AddressViewModel()
            {
                City = user.City,
                Country = user.Country,
                District = user.District,
                Name = user.Name,
                ZipCode = user.ZipCode,
                StreetAddress = user.StreetAddress


            };

            return View(model);
        }

        [Route("sifre-degistir"), HttpGet]
        public ActionResult ChangePassword(UpdatePasswordViewModel model)
        {

            if (CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            model.CurrentUser = CurrentUser;
            model.UserId = CurrentUser.Id;

            return View(model);
        }

        [Route("sifre-degistir"), HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePasswordPost(UpdatePasswordViewModel model)
        {
            if (model.UserId != CurrentUser.Id)
            {
                return new HttpUnauthorizedResult();
            }
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    model.Errors.Add("Şifreler eşleşmiyor");

                    return View(model);
                }

                var response = UserManager.ChangePasswordAsync(model.UserId, model.Password, model.NewPassword);

                if (response.Result.Succeeded)
                {
                    model.SuccessMessage = "Şifreniz başarıyla değiştirildi";
                }
                else
                {
                    var error = response.Result.Errors.FirstOrDefault();
                    switch (error)
                    {
                        case "Incorrect password.":
                            model.Errors.Add("Hatalı şifre");
                            break;
                    }

                }
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(t => t.ErrorMessage)).ToList();
                errors.ForEach(e => model.Errors.Add(e));
            }

            return View(model);
        }

    }
}