using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rankings.Web.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Registration Date")]
        [Required]
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
        public int ScoreFirstPlayer { get; set; }

        [Display(Name = "Score Second Player")]
        [Required]
        [Range(0, 100)]
        public int ScoreSecondPlayer { get; set; }

        public IEnumerable<SelectListItem> Players { get; set; }
        public IEnumerable<SelectListItem> OpponentPlayers { get; set; }
        public IEnumerable<SelectListItem> GameTypes { get; set; }
        public IEnumerable<SelectListItem> Venues { get; set; }
    }
}