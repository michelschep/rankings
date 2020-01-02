namespace Rankings.IntegrationTests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RankingFeatureContext
    {
        public bool? MarginOfVictory { get; set; }
        public int? Kfactor { get; set; }
        public int? N { get; set; }
        public int? InitialElo { get; set; }
        public int? MinimumRankingGames { get; set; }
    }
}