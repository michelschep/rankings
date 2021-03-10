using System;
using Rankings.Core.Entities;
using Rankings.Core.Projections;

namespace Rankings.Core.Interfaces
{
    public interface IGamesProjection: IGamesReporting
    {
        // Venues
        void CreateVenue(Venue venue);
        void Save(Venue entity);

        // Game Types
        void CreateGameType(GameType gameType);

        // Profiles
        void ActivateProfile(string email, string displayName);
        void CreateProfile(Profile profile);
        void DeactivateProfile(int id);
        void UpdateDisplayName(string emailAddress, string displayName);

        // Games
        [Obsolete("When we can get rid of Games table this method can be deleted")]
        void RegisterGame(Game game);
        void RegisterGame(GameProjection game);
        void RegisterDoubleGame(DoubleGame game);
        void Save(Game entity);
        void DeleteGame(int registrationDate);

        // Validations
        bool IsDisplayNameUnique(string displayName);
    }
}