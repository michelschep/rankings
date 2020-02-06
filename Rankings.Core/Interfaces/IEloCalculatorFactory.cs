namespace Rankings.Core.Interfaces
{
    public interface IEloCalculatorFactory
    {
        IEloCalculator Create(int year);
    }
}