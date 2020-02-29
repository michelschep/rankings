namespace Rankings.Core.Models
{
    public class EloConfiguration
    {
        public EloConfiguration(int kfactor, int n, bool withMarginOfVictory, int initialElo, int? numberOfGames)
        {
            Kfactor = kfactor;
            N = n;
            WithMarginOfVictory = withMarginOfVictory;
            InitialElo = initialElo;
            NumberOfGames = numberOfGames;
        }

        public int Kfactor { get; }
        public int N { get; }
        public bool WithMarginOfVictory { get; }
        public int InitialElo { get; }
        public int? NumberOfGames { get; }
        public bool JustNumbersForRanking { get; set; }
    }
}