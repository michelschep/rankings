using Rankings.Core.Entities;

namespace Rankings.Core.Interfaces
{
    public interface IGamesService: IGamesReporting
    {
        // Venues
        void CreateVenue(Venue venue);
        void Save(Venue entity);

        // Game Types
        void CreateGameType(GameType gameType);

        // Profiles
        void ActivateProfile(string email, string displayName);
        void CreateProfile(Profile profile);
        void UpdateDisplayName(string emailAddress, string displayName);

        // Games
        void RegisterGame(Game game);
        void Save(Game entity);
        void DeleteGame(int registrationDate);

        // Validations
        bool IsDisplayNameUnique(string displayName);
    }
}