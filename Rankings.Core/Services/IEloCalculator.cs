namespace Rankings.Core.Services
{
    public interface IEloCalculator
    {
        decimal CalculateDeltaPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
    }
}