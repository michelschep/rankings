using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Rankings.Web.Models
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class WhatIfModel
    {
        public decimal RatingPlayer1 { get; set; }
        public decimal RatingPlayer2 { get; set; }
        [Range(0, 10)]
        public int GameScore1 { get; set; }
        [Range(0, 10)]
        public int GameScore2 { get; set; }
        [DisplayName("Elo points to win or lose")]
        public decimal  Delta { get; set; }
        [DisplayName("Chance of winning one set")]
        public decimal ExpectedToWinSet { get; set; }
        [DisplayName("Chance of having specified result")]
        public decimal ExpectedToWinGame { get; set; }
    }
}