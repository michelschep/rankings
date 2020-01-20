namespace Rankings.Core.Services
{
    public interface IEloCalculatorFactory
    {
        IEloCalculator Create(int year);
    }
}