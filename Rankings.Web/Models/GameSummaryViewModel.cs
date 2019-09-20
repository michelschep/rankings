using System;
using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class GameSummaryViewModel
    {
        [Display(Name = "Code")]

        public DateTime RegistrationDate { get; set; }
        [Display(Name = "Type")]

        public string GameType { get; set; }
        public string Venue { get; set; }
        [Display(Name = "Player")]

        public string NameFirstPlayer { get; set; }
        [Display(Name = "Player")]

        public string NameSecondPlayer { get; set; }
        [Display(Name = "Score")]

        public int ScoreFirstPlayer { get; set; }
        [Display(Name = "Score")]

        public int ScoreSecondPlayer { get; set; }
    }
}