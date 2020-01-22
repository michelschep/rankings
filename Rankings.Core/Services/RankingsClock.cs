using System;

namespace Rankings.Core.Services
{
    public class RankingsClock : IRankingsClock
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}