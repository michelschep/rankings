using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Rankings.Core.Interfaces;

namespace Rankings.Web.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Id")]
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
    }

    public class UniqueDisplayNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var gameService = (IGamesService) validationContext.GetService(typeof(IGamesService));
            if (gameService == null)
                throw new ArgumentNullException(nameof(gameService));

            if (!gameService.IsDisplayNameUnique(value.ToString()))
                return new ValidationResult("Display name should be unique");

            return ValidationResult.Success;
        }
    }
}