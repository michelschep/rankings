using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public sealed class DoubleGamesForPeriodSpecification : BaseSpecification<DoubleGame>
    {
        public DoubleGamesForPeriodSpecification() 
            : base(g =>  (g.Score1 != 0 || g.Score2 !=0))
        {
            AddInclude(g => g.GameType);
            AddInclude(g => g.Player1Team1);
            AddInclude(g => g.Player2Team1);
            AddInclude(g => g.Player1Team2);
            AddInclude(g => g.Player2Team2);
            AddInclude(g => g.Venue);
        }
    }
}