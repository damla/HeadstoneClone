using Headstone.Framework.Models;
using Headstone.MetaData.API.Models.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class ListProductViewModel : BaseViewModel
    {
        public int ProductId { get; set; }

      
        public string ResellerName { get; set; }

        public string AnyPictures
        {
            get
            {
                if (Pictures.Count > 0)
                {
                    return "Evet";
                }
                else
                {
                    return "Hayır";
                }
            }
        }

        public string CategoryList
        {
            get
            {
                var categories = Categories.Select(a => a.Name).ToList();
                return String.Join(",", categories);
            }
        }

        public DateTime Created { get; set; }

        public EntityStatus Status { get; set; }

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

      
        public List<ProductPicture> Pictures { get; set; }

        public string Name { get; set; }

        public List<Category> Categories { get; set; }

        public List<Trademark> Trademarks { get; set; }

        public List<ProductProperty> Property { get; set; }
    }
}