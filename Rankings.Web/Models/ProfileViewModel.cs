using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } // set needed for POST edit

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

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