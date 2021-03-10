using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public sealed class AllGames : BaseSpecification<Game>
    {
        public AllGames() : base(g => true)
        {
            AddInclude(g => g.GameType);
            AddInclude(g => g.Player1);
            AddInclude(g => g.Player2);
            AddInclude(g => g.Venue);
        }
    }
}