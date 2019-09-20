using System;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Game : BaseEntity
    {
        public Profile Player1 { get; set; }

        public Profile Player2 { get; set; }

        public GameType GameType { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int Score1 { get; set; }
        
        public int Score2 { get; set; }

        public Venue Venue { get; set; }

        public string Comments { get; set; }
    }
}