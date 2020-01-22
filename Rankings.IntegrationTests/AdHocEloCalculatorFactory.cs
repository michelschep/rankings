using System;
using Rankings.Core.Services;

namespace Rankings.IntegrationTests
{
    public class AdHocEloCalculatorFactory : IEloCalculatorFactory
    {
        private readonly Func<IEloCalculator> _create;

        public AdHocEloCalculatorFactory(Func<IEloCalculator> create)
        {
            _create = create;
        }

        public IEloCalculator Create(int year)
        {
            return _create();
        }
    }
}