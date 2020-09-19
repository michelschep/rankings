using System;
using Rankings.Core.Interfaces;

namespace Rankings.IntegrationTests
{
    public class AdhocClock : IRankingsClock
    {
        private readonly Func<DateTime> _now;

        public AdhocClock(Func<DateTime> now)
        {
            _now = now;
        }

        public DateTime Now()
        {
            return _now();
        }
    }
}