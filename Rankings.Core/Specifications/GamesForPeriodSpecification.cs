﻿using System;
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
    
    public sealed class GamesForPlayerInPeriodSpecification : BaseSpecification<Game>
    {
        public GamesForPlayerInPeriodSpecification(string gameType, string emailAddres, DateTime startDate, DateTime endDate) 
            : base(g => g.GameType.Code == gameType 
                        && g.RegistrationDate >= startDate 
                        && g.RegistrationDate <= endDate
                        && g.Player1 != g.Player2
                        && (g.Player1.EmailAddress == emailAddres || g.Player2.EmailAddress == emailAddres) 
                        && (g.Score1 != 0 || g.Score2 !=0) 
                        )
        {
            AddInclude(g => g.GameType);
            AddInclude(g => g.Player1);
            AddInclude(g => g.Player2);
            AddInclude(g => g.Venue);
        }
    }
}
