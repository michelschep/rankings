namespace Rankings.Core.Services
{
    public class EloConfiguration
    {
        public EloConfiguration(int kfactor, int n, bool withMarginOfVictory, int initialElo, int numberOfGames = 0)
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
        public int NumberOfGames { get; }
    }
}