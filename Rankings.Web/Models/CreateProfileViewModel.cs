using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class CreateProfileViewModel
    {
        [Display(Name = "Email address")]
        [Required]
        public string EmailAddress { get; set; }

        [Display(Name = "Display Name")] [Required] 
        public string DisplayName { get; set; }
    }
}