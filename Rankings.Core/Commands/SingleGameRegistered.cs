﻿using System;

namespace Rankings.Core.Commands
{
    public class SingleGameRegistered
    {
        public DateTimeOffset RegistrationDate { get; set; }
        public Guid FirstPlayer { get; set; }
        public Guid SecondPlayer { get; set; }
        public int ScoreFirstPlayer { get; set; }
        public int ScoreSecondPlayer { get; set; }
        public string Venue { get; set; }
        public string GameType { get; set; }
        public Guid Identifier { get; set; }
        public string SetScoresFirstPlayer { get; set; }
        public string SetScoresSecondPlayer { get; set; }
    }
}