using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class RankingViewModel
    {
        [Display(Name = "")]
        public int Ranking { get; set; }

        [Display(Name = "Player")]
        public string NamePlayer { get; set; }

        [Display(Name = "Elo")]
        public int Points { get; set; }

        [Display(Name = "Win%")]
        public int WinPercentage { get; set; }

        [Display(Name = "SetW%")]
        public int SetWinPercentage { get; set; }

        [Display(Name = "History")]
        public List<char> History { get; set; }
    }
}