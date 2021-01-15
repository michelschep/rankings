using System;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class DoubleGame : BaseEntity
    {
        public string Identifier { get; set;  }
        public Profile Player1Team1 { get; set; }
        public Profile Player2Team1 { get; set; }
        public Profile Player1Team2 { get; set; }
        public Profile Player2Team2 { get; set; }
        public GameType GameType { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public Venue Venue { get; set; }
    }
}