using Lidia.Identity.API.Models;

namespace Headstone.AI.Models.ViewModels
{
    public class ResetPasswordNotificationViewModel : BaseViewModel
    {
        public User User { get; set; }

        public string SiteRoot { get; set; }

        public string SecurityStamp { get; set; }

        public string ResetToken { get; set; }
    }
}