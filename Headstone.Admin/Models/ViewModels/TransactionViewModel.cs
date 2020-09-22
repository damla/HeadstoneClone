using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Headstone.Framework.Models;
using Lidia.Identity.API.Models;

namespace Headstone.AI.Models.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        
        public int TransactionId { get; set; }

        public string RefNumber { get; set; }

        public int UserId { get; set; }

        public int OrderId { get; set; }

        public int OwnerId { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return (Firstname + " " + LastName); }}

        public User User { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }

        public string TransactionCode { get; set; }

        public string Account { get; set; }

        public int Installments { get; set; }

        public decimal? InstallmentInterest { get; set; }

        public decimal? InstallmentAmount { get; set; }

        public string Gateway { get; set; }

        public string Channel { get; set; }

        public string ChannelCode { get; set; }

        public string ChannelAccount { get; set; }

        public string ChannelResponse { get; set; }

        public string ChannelRefCode { get; set; }

        public string ChannelAuthCode { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string SecurityLevel { get; set; }


        public DateTime TransactionDate { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public EntityStatus Status { get; set; }
    }
}