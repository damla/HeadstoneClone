using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.MetaData.API.Models;
using Headstone.MetaData.API.Models.Live;

namespace Headstone.AI.Models.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        [Display(Name = "Kategori Adı")]

        public string Name { get; set; }

        public int CategoryId { get;  set; }

        [Display(Name = "Kategori Açıklaması")]
        public string LongDescription { get;  set; }

        [Display(Name = "Kategori Tipi")]
        public CategoryType Type { get; set; }

        [Display(Name = "Üst Kategori Adı")]
        public int ParentId { get;  set; }

        public List<CategoryProduct> Products { get;  set; }

        [Display(Name = "Açıklama")]
        public string ShortDescription { get;  set; }

        public string Code { get;  set; }

        public string CRMCode { get;  set; }

        public int SortOrder { get;  set; }

        public string IdPath { get; set; }

        public string NamePath { get; set; }

        public bool IsLeaf { get; set; }

        [Display(Name = "Durum")]
        public EntityStatus Status { get;  set; }
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
                    case EntityStatus.Deleted:
                        return "Silinmiş";
                    case EntityStatus.Freezed:
                        return "Dondurulmuş";
                    case EntityStatus.Blocked:
                        return "Bloklandı";
                    case EntityStatus.Draft:
                        return "Taslak";
                    case EntityStatus.Test:
                        return "Test";
                    case EntityStatus.Unknown:
                        return "Bilinmiyor";
                    default:
                        return "";
                }
            }
        }

        public SelectList Types { get;  set; }

        public List<SelectListItem> ParentList { get;  set; } = new List<SelectListItem>();

        public SelectList Statuses { get;  set; }

        public List<Category> Children { get; set; }

        public List<CategoryProperty> Properties { get; set; }

        public List<CategoryPicture> Pictures { get; set; }

        public Category Parent { get; set; }

        public List<CategoryTag> Tags { get; set; }

    }
}