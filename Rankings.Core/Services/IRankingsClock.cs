using System;

namespace Rankings.Core.Services
{
    public interface IRankingsClock
    {
        DateTime Now();
    }
}