using System;
using System.ComponentModel.DataAnnotations;
using Rankings.Core.Interfaces;

namespace Rankings.Web.Models
{
    public class UniqueDisplayNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var gameService = (IGamesProjection) validationContext.GetService(typeof(IGamesProjection));
            if (gameService == null)
                throw new ArgumentNullException(nameof(gameService));

            if (!gameService.IsDisplayNameUnique(value.ToString()))
                return new ValidationResult("Display name should be unique");

            return ValidationResult.Success;
        }
    }

    public class NoPointsDrawIsNotAllowedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var game = (GameViewModel)validationContext.ObjectInstance;

            if (game.ScoreFirstPlayer == 0 && game.ScoreSecondPlayer == 0)
            {
                return new ValidationResult("Draw without sets is not allowed");
            }

            return ValidationResult.Success;
        }
    }
}