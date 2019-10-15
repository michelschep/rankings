using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rankings.Web.Models
{
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
        [DisplayName("Change of winning one set")]
        public decimal ExpectedToWinSet { get; set; }
        [DisplayName("Change of winning game with entered score")]
        public decimal ExpectedToWinGame { get; set; }
    }
}