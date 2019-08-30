using System.Collections.Generic;

namespace Rankings.Web.Controllers
{
    public interface IRankingService
    {
        IEnumerable<Profile> Profiles();
        void ActivateProfile(string email, string displayName);
        Profile ProfileFor(string email);
        void UpdateDisplayName(string emailAddress, string displayName);
    }
}