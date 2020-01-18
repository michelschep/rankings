using System;

namespace Rankings.Core.Services
{
    public class StatGame
    {
        public int Score1 { get; set; }
        public int Score2 { get; set;  }
        public decimal? Delta1 { get; set; }
        public decimal? Delta2 { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal EloPlayer2 { get; set; }

        public StatGame(in int score1, in int score2, decimal? delta1 = null, decimal? delta2 = null)
        {
            Score1 = score1;
            Score2 = score2;
            Delta1 = delta1;
            Delta2 = delta2;
        }
    }
}