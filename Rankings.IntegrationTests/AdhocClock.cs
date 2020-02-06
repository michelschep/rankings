using System;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;

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