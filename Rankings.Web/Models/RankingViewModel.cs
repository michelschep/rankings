using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Rankings.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class RankingViewModel
    {
        [Display(Name = "")]
        public int Ranking { get; set; }

        [Display(Name = "Player")]
        public string NamePlayer { get; set; }

        [Display(Name = "Elo")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Points { get; set; }

        [Display(Name = "Win%")]
        public string WinPercentage { get; set; }

        [Display(Name = "SetW%")]
        public string SetWinPercentage { get; set; }

        [Display(Name = "History")]
        public List<char> History { get; set; }

        public int NumberOfGames { get; set; }

        [Display(Name = "BWS")]
        public int RecordWinningStreak { get; set; }

        [Display(Name = "BES")]
        public int RecordEloStreak { get; set; }

        [Display(Name = "CWS")]
        public int CurrentWinningStreak { get; set; }

        [Display(Name = "CES")]
        public int CurrentEloStreak { get; set; }

        [Display(Name = "Skalp Streak")]
        public int SkalpStreak { get; set; }
        public int Goat { get; set; }

        [Display(Name = "Time #1")]
        public string TimeNumberOne { get; set; }

        public string EmailAddress { get; set; }
    }
}