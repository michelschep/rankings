using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Rankings.Web.Controllers;

namespace Rankings.Web.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Id")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public int Id { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [ReadOnly(true)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; } // set needed for POST edit

        [Display(Name = "Display Name")]
        [StringLength(30)]
        [Required]
        [DataType(DataType.Text)]
        [UniqueDisplayName]
        public string DisplayName { get; set; }

        public IEnumerable<StreakViewModel> Streaks { get; set; }
    }
}