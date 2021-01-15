using System;
using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public sealed class SpecificGame : BaseSpecification<Game>
    {
        public SpecificGame(string id) : base(g => g.Identifier == id)
        {
            AddInclude(g => g.GameType);
            AddInclude(g => g.Player1);
            AddInclude(g => g.Player2);
            AddInclude(g => g.Venue);
        }
    }

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