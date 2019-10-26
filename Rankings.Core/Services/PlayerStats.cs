namespace Rankings.Core.Services
{
    public class PlayerStats
    {
        public decimal Ranking { get; set; }
        public int NumberOfGames { get; set; }
        public int NumberOfWins { get; set; }
        public int NumberOfSets { get; set; }
        public int NumberOfSetWins { get; set; }
        public string History { get; set; }
        public decimal WinPercentage { get; set; }
        public decimal SetWinPercentage { get; set; }
    }
}