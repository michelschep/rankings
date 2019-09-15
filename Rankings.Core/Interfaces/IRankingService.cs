using System.Collections.Generic;
using Rankings.Core.Entities;

namespace Rankings.Core.Interfaces
{
    public interface IRankingService
    {
        IEnumerable<Profile> Profiles();
        void ActivateProfile(string email, string displayName);
        Profile ProfileFor(string email);
        void UpdateDisplayName(string emailAddress, string displayName);
        IEnumerable<GameType> GameTypes();
    }
}