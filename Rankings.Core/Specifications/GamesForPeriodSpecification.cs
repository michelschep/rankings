using System;
using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public sealed class GamesForPeriodSpecification : BaseSpecification<Game>
    {
        public GamesForPeriodSpecification(string gameType, DateTime startDate, DateTime endDate) 
            : base(g => g.GameType.Code == gameType 
                        && g.RegistrationDate >= startDate 
                        && g.RegistrationDate <= endDate
                        // TODO quick fix. Do not show same player games. 
                        && g.Player1 != g.Player2
                        )
        {
            AddInclude(g => g.GameType);
            AddInclude(g => g.Player1);
            AddInclude(g => g.Player2);
            AddInclude(g => g.Venue);
        }
    }
}
