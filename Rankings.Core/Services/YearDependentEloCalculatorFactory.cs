using Rankings.Core.Interfaces;

namespace Rankings.Core.Services
{
    public class YearDependentEloCalculatorFactory : IEloCalculatorFactory
    {
        public IEloCalculator Create(int year)
        {
            if (year == 2020 || year == 2021 || year == 2022)
                return new EloCalculatorVersion2020();

            if (year == 2019)
                return new EloCalculatorVersion2019();

            return new DefaultEloCalculator();
        }
    }
}