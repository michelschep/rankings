using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rankings.Web.Models
{
    // TODO setter needed?
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class GameViewModel: IValidatableObject
    {
        public int Id { get; set; }
        
        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }

        [Display(Name = "Game Type")]
        [Required]
        public string GameType { get; set; }

        [Display(Name = "Venue")]
        [Required]
        public string Venue { get; set; }

        [Display(Name = "First Player")]
        [Required]
        public string NameFirstPlayer { get; set; }

        [Display(Name = "Second Player")]
        [Required]
        public string NameSecondPlayer { get; set; }

        [Display(Name = "Score First Player")]
        [Required]
        [Range(0, 100)]
        [NoPointsDrawIsNotAllowed]
        public int ScoreFirstPlayer { get; set; }

        [Display(Name = "Score Second Player")]
        [Required]
        [Range(0, 100)]
        public int ScoreSecondPlayer { get; set; }

        public IEnumerable<SelectListItem> Players { get; set; }
        public IEnumerable<SelectListItem> OpponentPlayers { get; set; }
        public IEnumerable<SelectListItem> GameTypes { get; set; }
        public IEnumerable<SelectListItem> Venues { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ScoreFirstPlayer == 0 && ScoreSecondPlayer == 0)
            {
                yield return new ValidationResult("Game result for table tennis cannot be 0-0");
            }
        }
    }
}