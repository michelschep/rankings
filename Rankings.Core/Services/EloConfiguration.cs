namespace Rankings.Core.Services
{
    public class EloConfiguration
    {
        public EloConfiguration(int kfactor, int n, bool withMarginOfVictory, int initialElo)
        {
            Kfactor = kfactor;
            N = n;
            WithMarginOfVictory = withMarginOfVictory;
            InitialElo = initialElo;
        }

        public int Kfactor { get; }
        public int N { get; }
        public bool WithMarginOfVictory { get; }
        public int InitialElo { get; }
    }
}