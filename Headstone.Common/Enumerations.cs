using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Headstone.Common
{
    public enum MessageType
    {
        Standard = 0,

        Admin = 1,

        Verdict = 2,

        AdminOnly = 3,

        Document = 4
    }

    public enum CommentType
    {
        Standard = 0,

        AdminOnly = 99
    }

    public enum GeoLocaitonTypes
    {
        Country = 0,
        Region = 1,
        City = 2,
        District = 3,
        Neighborhood = 4
    }

   
    // Authorization
    public enum AuthorizationStatus
    {
        Invalid_Key = -2,

        No_Valid_Request = -1,

        Not_Authorized = 0,

        Authorized = 1
    }

    // Services
    public enum ServiceResponseTypes
    {
        Timeout = -999,

        Error = -99,

        Declined = -2,

        Pending = -1,

        Unknown = 0,

        Completed = 1,

        Redirected = 2,

        Approved = 3,

        Success = 4
    }

    [Serializable]
    public enum ServiceResponseSources
    {
        Undefined = 0,

        MsSQL = 1,

        Index = 2,

        Cache = 3,

        NoSql = 4,

        ThirdParty = 5,

        FileSystem = 6
    }

    public enum HeadstoneServiceResponseCodes
    {
        // General
        Cancelled = -999,

        General_Exception = -300,

        Invalid_Request = -301,

        Invalid_Channel = -302,

        Invalid_Option = -303,

        Authorization_Failed = -304,

        Invalid_Request_Parameters = -305,

        Request_Completed_WithErrors = -200,

        Request_Successfuly_Completed = 200
    }

    // Framework
    public enum EntityStatus : short
    {
        [Display(Name = "Bilinmiyor")]
        Unknown = (short)-999,
        [Display(Name = "Silindi")]
        Deleted = (short)-99,
        [Display(Name = "Engellendi")]
        Blocked = (short)-12,
        [Display(Name = "Donduruldu")]
        Freezed = (short)-11,
        [Display(Name = "Test")]
        Test = (short)-9,
        [Display(Name = "Pasif")]
        Passive = (short)-1,
        [Display(Name = "Taslak")]
        Draft = (short)0,
        [Display(Name ="Aktif")]
        Active = (short)1
    }

    // Data 
    public enum CommentEntityType
    {
        Product = 10,
        
        Offer = 20,
        
        Tender = 30,
        
        Category = 40,

        Trademark = 50,

        Reseller = 60,

        Order = 70,

        Payment = 80,

        User = 90,
        
        Organization = 100
    }

    public enum TagType
    {
        Administrative = 0,

        Attribute = 1,

        Badge = 2
    }

    // Ticket
    public enum TicketType
    {
        [Display(Name = "Bilgi")]
        Information = 10,

        [Display(Name = "Öneri")]
        Proposal = 20,

        [Display(Name = "İstek")]
        Demand = 30,

        [Display(Name = "Şikayet")]
        Complaint = 40
    }

    public enum TicketEntityType
    {
        [Display(Name = "Başvuru süreci")]
        Reseller_Application = 10,
     
        [Display(Name ="Sipariş süreci")]
        Order = 20,

        [Display(Name = "Talep süreci")]
        Tender = 30,

        [Display(Name = "Fırsat süreci")]
        Offer = 40,
        [Display(Name = "Anlaşmazlık süreci")]
        Dispute = 50,
        
        [Display(Name = "Teklif süreci")]
        TenderBid = 60,

        [Display(Name = "Ürün")]
        Product = 70,

        [Display(Name ="Yorum ve puan süreci")]
        Comment = 80
    }

    public enum TicketStatus
    {

        Open = 2,

        Pending = 3,

        Resolved = 4,

        Closed = 5
    }

    public enum TicketPriority
    {

        Low = 1,

        Medium = 2,

        High = 3,

        Urgent = 4
    }

    public enum DiscountType
    {
        Percentage = 10,

        Amount = 20
    }
}
