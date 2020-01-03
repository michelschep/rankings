namespace Rankings.Core.Services
{
    internal class StatGame
    {
        public int Score1 { get; }
        public int Score2 { get; }
        public decimal? Delta1 { get; }
        public decimal? Delta2 { get; }

        public StatGame(in int score1, in int score2, decimal? delta1 = null, decimal? delta2 = null)
        {
            Score1 = score1;
            Score2 = score2;
            Delta1 = delta1;
            Delta2 = delta2;
        }
    }
}