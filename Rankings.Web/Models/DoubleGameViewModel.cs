using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rankings.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DoubleGameViewModel: IValidatableObject
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

        [Display(Name = "Winners")]
        [Required]
        public string NameFirstPlayerFirstTeam { get; set; }

        [Required]
        public string NameSecondPlayerFirstTeam { get; set; }

        [Display(Name = "Losers")]
        [Required]
        public string NameFirstPlayerSecondTeam { get; set; }

        [Required]
        public string NameSecondPlayerSecondTeam { get; set; }

        [Required]
        [Range(0, 100)]
        public int ScoreFirstTeam { get; set; }

        [Required]
        [Range(0, 100)]
        public int ScoreSecondTeam{ get; set; }

        public IEnumerable<SelectListItem> Players { get; set; }
        public IEnumerable<SelectListItem> GameTypes { get; set; }
        public IEnumerable<SelectListItem> Venues { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var players = new List<string>()
            {
                NameFirstPlayerFirstTeam, NameSecondPlayerFirstTeam,
                NameFirstPlayerSecondTeam, NameSecondPlayerSecondTeam
            };

            if (players.Distinct().Count() != 4)
            {
                yield return new ValidationResult("Duplicate players are now allowed");
            }

            if (ScoreFirstTeam == 0 && ScoreSecondTeam == 0)
            {
                yield return new ValidationResult("Game result for table tennis cannot be 0-0");
            }
        }
    }
}