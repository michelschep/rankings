using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class GameSummaryViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Registration Date")]
        public string RegistrationDate { get; set; }

        [Display(Name = "Type")]
        public string GameType { get; set; }

        [Display(Name = "Venue")]
        public string Venue { get; set; }
        
        [Display(Name = "Player")]
        public string NameFirstPlayer { get; set; }
        
        [Display(Name = "Player")]
        public string NameSecondPlayer { get; set; }
        
        [Display(Name = "Score")]
        public int ScoreFirstPlayer { get; set; }
        
        [Display(Name = "Score")]
        public int ScoreSecondPlayer { get; set; }

        public bool IsEditable { get; set; }
    }
}