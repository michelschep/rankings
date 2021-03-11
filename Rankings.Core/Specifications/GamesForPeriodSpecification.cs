using System;
using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    [Obsolete]
    public sealed class GamesForPeriodSpecification : BaseSpecification<Game>
    {
        public GamesForPeriodSpecification(string gameType, DateTime startDate, DateTime endDate) 
            : base(g => g.GameType.Code == gameType 
                        && g.RegistrationDate >= startDate 
                        && g.RegistrationDate <= endDate
                        // TODO quick fix. Do not show same player games. 
                        && g.Player1 != g.Player2
                        // TODO quick fix 0-0 is not allowed for table tennis 
                        && (g.Score1 != 0 || g.Score2 !=0) 
                        )
        {
            AddInclude(g => g.GameType);
            AddInclude(g => g.Player1);
            AddInclude(g => g.Player2);
            AddInclude(g => g.Venue);
        }
    }
    
    public sealed class ProjectionGamesForPeriodSpecification : BaseSpecification<GameProjection>
    {
        public ProjectionGamesForPeriodSpecification(string gameType, DateTime startDate, DateTime endDate) 
            : base(g => g.GameType == gameType 
                        && g.RegistrationDate >= startDate 
                        && g.RegistrationDate <= endDate
                        // TODO quick fix. Do not show same player games. 
                        && g.FirstPlayerId != g.SecondPlayerId
                        // TODO quick fix 0-0 is not allowed for table tennis 
                        && (g.Score1 != 0 || g.Score2 !=0) 
                        )
        {
        }
    }
}
