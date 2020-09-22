using Lidia.Identity.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.ViewModels
{
    public class UserConsentViewModel : BaseViewModel
    {
        public UserConsentType ConsentType { get; set; }

        public string UserIp { get; set; }

        public string Name
        {
            get
            {
                switch (ConsentType)
                {
                    case UserConsentType.Clarification_Consent_Given:
                        return "Kontrat İzini";
                    case UserConsentType.Clarification_Consent_Withdrawn:
                        return "Kontrat İzini";
                    case UserConsentType.Data_Processing_Consent_Given:
                        return "Veri İşleme İzini";
                    case UserConsentType.Data_Processing_Consent_Withdrawn:
                        return "Veri İşleme İzini";
                    case UserConsentType.General_Marketing_Consent_Given:
                        return "Genel Pazarlama İzini";
                    case UserConsentType.General_Marketing_Consent_Withdrawn:
                        return "Genel Pazarlama İzini";
                    case UserConsentType.SMS_Marketing_Consent_Given:
                        return "SMS İzini";
                    case UserConsentType.SMS_Marketing_Consent_Withdrawn:
                        return "SMS İzini";
                    case UserConsentType.Email_Marketing_Consent_Given:
                        return "Email İzini";
                    case UserConsentType.Email_Marketing_Consent_Withdrawn:
                        return "Email İzini";
                    case UserConsentType.Phone_Marketing_Consent_Given:
                        return "Telefon İzini";
                    case UserConsentType.Phone_Marketing_Consent_Withdrawn:
                        return "Telefon İzini";
                    default:
                        return "";
                        //}
                }
            }
        }

        public DateTime Approved { get; set; }

        public DateTime Rejected { get; set; }

        public DateTime Revoked { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUntil { get; set; }

        public DateTime Updated
        {
            get
            {
                var list = new List<DateTime> { Approved, Rejected, Revoked };
                return list.Max();
            }
        }

        public string State
        {
            get
            {
                var list = new List<DateTime> { Approved, Rejected, Revoked };
                var index=list.IndexOf(list.Max());
                switch (index)
                {
                    case 0:
                        return "Kabul edildi";
                    case 1:
                        return "Reddedildi";
                    case 2:
                        return "İptal edildi";
                    default:
                        return "";
                }
            }
        }

    }
}