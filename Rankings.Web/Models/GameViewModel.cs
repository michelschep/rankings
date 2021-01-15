using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rankings.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class GameViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Registration Date")] public string RegistrationDate { get; set; }

        [Display(Name = "Game Type")]
        [Required]
        public string GameType { get; set; }

        [Display(Name = "Venue")] [Required] public string Venue { get; set; }

        [Display(Name = "First Player")]
        [Required]
        public string NameFirstPlayer { get; set; }

        [Display(Name = "Second Player")]
        [Required]
        public string NameSecondPlayer { get; set; }

        [Display(Name = "Score First Player")]
        [Required]
        [NoPointsDrawIsNotAllowed]
        public int ScoreFirstPlayer { get; set; }
        public string ScoreFirstPlayerSet1 { get; set; }
        public string ScoreFirstPlayerSet2 { get; set; }
        public string ScoreFirstPlayerSet3 { get; set; }
        public string ScoreFirstPlayerSet4 { get; set; }
        public string ScoreFirstPlayerSet5 { get; set; }

        [Display(Name = "Score Second Player")]
        [Required]
        [Range(0, 100)]
        public int ScoreSecondPlayer { get; set; }
        public string ScoreSecondPlayerSet1 { get; set; }
        public string ScoreSecondPlayerSet2 { get; set; }
        public string ScoreSecondPlayerSet3 { get; set; }
        public string ScoreSecondPlayerSet4 { get; set; }
        public string ScoreSecondPlayerSet5 { get; set; }

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

            if (!string.IsNullOrEmpty(ScoreSecondPlayerSet1))
            {
                var numberOfSets = ScoreFirstPlayer + ScoreSecondPlayer;
                var sets1 = new List<string>
                {
                    this.ScoreFirstPlayerSet1,
                    this.ScoreFirstPlayerSet2,
                    this.ScoreFirstPlayerSet3,
                    this.ScoreFirstPlayerSet4,
                    this.ScoreFirstPlayerSet5
                };

                var index = 1;
                foreach (var s in sets1.Take(numberOfSets))
                {
                    if (!int.TryParse(s, out _))
                    {
                        yield return new ValidationResult($"Value set {index} for {NameFirstPlayer} is not a valid number");
                        yield break;
                    }

                    ++index;
                }
                foreach (var s in sets1.Skip(numberOfSets))
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        yield return new ValidationResult($"Value set {index} for {NameFirstPlayer} should be empty");
                        yield break;
                    }

                    ++index;
                }

                var sets2 = new List<string>
                {
                    this.ScoreSecondPlayerSet1,
                    this.ScoreSecondPlayerSet2,
                    this.ScoreSecondPlayerSet3,
                    this.ScoreSecondPlayerSet4,
                    this.ScoreSecondPlayerSet5
                };
                index = 1;
                foreach (var s in sets2.Take(numberOfSets))
                {
                    if (!int.TryParse(s, out _))
                    {
                        yield return new ValidationResult($"Value set {index} for {NameSecondPlayer} is not a valid number");
                        yield break;
                    }

                    ++index;
                }
                foreach (var s in sets2.Skip(numberOfSets))
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        yield return new ValidationResult($"Value set {index} for {NameFirstPlayer} should be empty");
                        yield break;
                    }

                    ++index;
                }

                var win1 = 0;
                var win2 = 0;
                for (var set = 0; set < numberOfSets; ++set)
                {
                    if (int.Parse(sets1[set]) > int.Parse(sets2[set]))
                        ++win1;
                    else 
                        ++win2;
                }

                if (win1 != this.ScoreFirstPlayer || win2 != this.ScoreSecondPlayer)
                    yield return new ValidationResult($"Game result does not correspond to set results");
            }
        }
    }

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