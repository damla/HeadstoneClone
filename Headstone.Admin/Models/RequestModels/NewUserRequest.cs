using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Headstone.AI.Models.RequestModels
{
    public class NewUserRequest
    {

        public string FirstName{ get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string AccountStatus { get; set; }
        public int SelectedStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public HttpPostedFileBase UploadedAvatar { get; set; }

    }
}