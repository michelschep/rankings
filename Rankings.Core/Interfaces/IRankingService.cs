using System.Collections.Generic;
using Rankings.Core.Entities;
using Rankings.Core.Services;

namespace Rankings.Core.Interfaces
{
    public interface IRankingService
    {
        IEnumerable<Venue> GetVenues();
        IEnumerable<Profile> Profiles();
        IEnumerable<GameType> GameTypes();
        IEnumerable<Game> Games();
        Profile ProfileFor(string email);

        void CreateGameType(GameType gameType);
        void ActivateProfile(string email, string displayName);
        void UpdateDisplayName(string emailAddress, string displayName);
        void RegisterGame(Game game);
        void CreateVenue(Venue venue);
        void DeleteGame(int registrationDate);
        void CreateProfile(Profile profile);
        void Save(Game entity);

        Dictionary<Profile, PlayerStats> Ranking();
        decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2);
        decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2, int numberOfSets);
    }
}