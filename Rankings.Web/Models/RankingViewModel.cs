using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class RankingViewModel
    {
        [Display(Name = "Ranking")]
        public int Ranking { get; set; }

        [Display(Name = "Player")]
        public string NamePlayer { get; set; }

        [Display(Name = "Elo")]
        public int Points { get; set; }

        [Display(Name = "Win%")]
        public int WinPercentage { get; set; }

        [Display(Name = "SetWin%")]
        public int SetWinPercentage { get; set; }

//        [Display(Name = "History")]
//        public string History { get; set; }
    }
}