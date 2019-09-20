using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
    public class RankingViewModel
    {
        [Display(Name = "Ranking")]
        public int Ranking { get; set; }

        [Display(Name = "Player")]
        public string NamePlayer { get; set; }

        [Display(Name = "Points")]
        public int Points { get; set; }
    }
}