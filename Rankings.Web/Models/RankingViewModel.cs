using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class RankingViewModel
    {
        [Display(Name = "")]
        public string Ranking { get; set; }

        [Display(Name = "Player")]
        public string NamePlayer { get; set; }

        [Display(Name = "Elo")]
        public string Points { get; set; }

        [Display(Name = "Win%")]
        public string WinPercentage { get; set; }

        [Display(Name = "SetW%")]
        public string SetWinPercentage { get; set; }

        [Display(Name = "History")]
        public List<char> History { get; set; }
    }
}