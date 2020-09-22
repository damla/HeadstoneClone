using Headstone.Framework.Models;
using Headstone.MetaData.API.Models.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public int ProductId { get; set; }

        public string Code { get; set; }

        public string ERPCode { get; set; }

        public string EANCode { get; set; }

        public decimal TaxRate { get; set; }

        public string SourceProductId { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string ResellerName { get; set; }

        public DateTime Created { get; set; }

        public EntityStatus Status { get; set; }

        public string TrademarkName { get; set; }

        //public string AnyPictures
        //{
        //    get
        //    {
        //        if (Pictures.Count > 0)
        //        {
        //            return "Evet";
        //        }
        //        else
        //        {
        //            return "Hayır";
        //        }
        //    }
        //}

        //public string CategoryList { get; set; }

        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case EntityStatus.Active:
                        return "Aktif";
                    case EntityStatus.Passive:
                        return "Pasif";
                    case EntityStatus.Blocked:
                        return "Bloklandı";
                    case EntityStatus.Deleted:
                        return "Silindi";
                    case EntityStatus.Draft:
                        return "Taslak";
                    case EntityStatus.Freezed:
                        return "Donduruldu";
                    case EntityStatus.Test:
                        return "Test";
                    case EntityStatus.Unknown:
                        return "Bilinmiyor";
                    default:
                        return "";
                }
            }
        }

        public List<ProductVariant> Variants { get; set; }

        public List<ProductTag> Tags { get; set; }

        public List<ProductPicture> Pictures { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public List<Category> Categories { get; set; }

        public List<Trademark> Trademarks { get; set; }

        public List<ProductProperty> Property { get; set; }

        public Product Product { get; set; }
    }
}