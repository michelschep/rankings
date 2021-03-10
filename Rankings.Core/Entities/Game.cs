using System;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Game : BaseEntity
    {
        public string Identifier { get; set; }
        public Profile Player1 { get; set; }
        public Profile Player2 { get; set; }
        public GameType GameType { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public Venue Venue { get; set; }
        public string SetScores1 { get; set; }
        public string SetScores2 { get; set; }
    }
    
    public class Event : BaseEntity
    {
        public string Identifier { get; set; }
        public int Index { get; set; }
        public DateTime CreationDate { get; set; }
        public string Type { get; set; }
        public string Body { get; set; }
    }
}