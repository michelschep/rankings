using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Email Address")]
        public string EmailAddress { get; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; }

        public ProfileViewModel()
        {
            // needed for asp.net mvc core
        }

        public ProfileViewModel(string emailAddress, string displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }
    }
}