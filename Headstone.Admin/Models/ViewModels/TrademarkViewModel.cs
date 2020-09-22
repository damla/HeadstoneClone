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
    public class TrademarkViewModel : BaseViewModel
    {
        [Display(Name = "Marka Adı")]
        public string Name { get; set; }

        public int TrademarkId { get; set; }

        [Display(Name = "Marka Açıklaması")]
        public string LongDescription { get; set; }

        [Display(Name = "Üst Marka Adı")]
        public int ParentId { get; set; }

        public List<TrademarkProduct> Products { get; set; }

        [Display(Name = "Ek Bilgi")]
        public string ShortDescription { get; set; }

        public string Code { get; set; }

        public string ERPCode { get; set; }

        public int SortOrder { get; set; }

        [Display(Name = "Durum")]
        public EntityStatus Status { get; set; }

        public SelectList Types { get; set; }

        public List<SelectListItem> ParentList { get; set; } = new List<SelectListItem>();

        public SelectList Statuses { get; set; }

        public List<Trademark> Children { get; set; }

        public List<TrademarkProperty> Properties { get; set; }

        public List<TrademarkPicture> Pictures { get; set; }

        public Trademark Parent { get; set; }

        public List<TrademarkTag> Tags { get; set; }

    }
}