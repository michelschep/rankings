using System;

namespace Rankings.Core.Services
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal d, int precision = 0)
        {
            return Math.Round(d, precision, MidpointRounding.AwayFromZero);
        }
    }
}