using System;
using System.ComponentModel.DataAnnotations;
using Rankings.Core.Interfaces;

namespace Rankings.Web.Models
{
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