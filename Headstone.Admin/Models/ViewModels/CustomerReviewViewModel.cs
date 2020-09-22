using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Headstone.Common;
using Headstone.MetaData.API.Models;
using Headstone.MetaData.API.Models.Live;

namespace Headstone.AI.Models.ViewModels
{
    public class CustomerReviewViewModel : BaseViewModel
    {
        public int CommentId { get; set; }

        public int? ParentId { get; set; }

        public int? UserId { get; set; }

        public string RelatedUserName { get; set; }

        public string RelatedReseller { get; set; }

        public int? OrganizationId { get; set; }

        public string RelatedDataEntityType { get; set; }

        public int RelatedDataEntityId { get; set; }

        public CommentType Type { get; set; }

        public int? Rating { get; set; }

        public string Body { get; set; }

        public string Created { get; set; }

        public DateTime? Updated { get; set; }

        public Headstone.Framework.Models.EntityStatus Status { get; set; }
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case Headstone.Framework.Models.EntityStatus.Active:
                        return "Aktif";
                    case Headstone.Framework.Models.EntityStatus.Passive:
                        return "Pasif";
                    case Headstone.Framework.Models.EntityStatus.Deleted:
                        return "Silinmiş";
                    case Headstone.Framework.Models.EntityStatus.Freezed:
                        return "Donduruldu";
                    case Headstone.Framework.Models.EntityStatus.Blocked:
                        return "Bloklandı";
                    default:
                        return "";
                }
            }
        }
    }
}