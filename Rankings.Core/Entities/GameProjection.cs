using System;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class GameProjection: BaseEntity
    {
        public string Identifier { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string GameType { get; set; }
        public string Venue { get; set; }
        public string FirstPlayerId { get; set; }
        public string FirstPlayerName { get; set; }
        public string SecondPlayerId { get; set; }
        public string SecondPlayerName { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public string Status { get; set; }
        public string EloFirstPlayer { get; set; }
        public string EloSecondPlayer { get; set; }
    }
}