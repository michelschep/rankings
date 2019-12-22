using Rankings.Core.Entities;

namespace Rankings.Core.Services.ToBeObsolete
{
    public class ObsoleteNewPlayerStats
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Profile Profile { get; set; }
        public int Rating { get; set; }
        public int TimeNumberOne { get; set; }
        public int NumberOfGamesNumberOne { get; set; }
        public int GoatRating { get; set; }
        public int NumberOfCleanSheets { get; set; }
        public int NumberOfGames { get; set; }
        public int EloWonOpponent { get; set; }
        public int Won { get; set; }
        public int Lost { get; set; }
        public int EloLostOpponent { get; set; }
        public int WonStreak { get; set; }
        public int WonStreakRecord { get; set; }
    }
}