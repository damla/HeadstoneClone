using Lidia.Identity.API.Models;
using Lidia.Identity.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Headstone.AI.Models.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public int Id { get; set; }


        [Display(Name = "İsim")]
        public string Firstname { get; set; }


        [Display(Name = "Soyad")]
        public string Lastname { get; set; }


        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        public string Fullname { 
            get
            {
                return Firstname + " " + Lastname;
            }
        }


        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; }

        public SelectList GenderList { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Telefon")]
        [Phone]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Cep Telefon")]
        [Phone]
        [MaxLength(10)]
        public string MobileNumber { get; set; }

        [Display(Name = "Şifre")]
        public string Password { get; set; }

        public AccountStatus AccountStatus { get; set; }

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
                    default:
                        return "";
                }
            }
        }

        public string ResellerName { get; set; }

        public SelectList AccountStatusList { get; set; }

        [Display(Name = "Durum")]
        public int SelectedStatus { get; set; }

        [Display(Name ="Şehir")]
        public string City { get; set; }

        public SelectList CityList { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public EntityStatus Status { get; set; }

        public int RoleId { get; set; }

        public string SecurityStamp { get; set; }

        public string RolesString
        {
            get
            {
                return String.Join(", ", Roles.Select(r => r.RoleName));
            }
        }

        public SelectList RoleList { get; set; }

        public List<string> Consents { get; set; } = new List<string>();

        public string PermissionsString { 
            get
            {
                return string.Join(", ", Consents);
            }
        }
        public List<UserRole> Roles { get; set; } = new List<UserRole>();

        public List<UserAddress> Addresses { get; set; }

        public List<UserBillingInfo> BillingInfos { get; set; }

        public string PictureUrl { get; set; }

        public string AvatarBase64 { get; set; }

        public string AvatarMimeType { get; set; }
    }
}