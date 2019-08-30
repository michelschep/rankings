using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Services
{
    public class TestRankingService : IRankingService
    {
        private readonly List<Profile> _repository;

        public TestRankingService()
        {
            _repository = new List<Profile>();
        }

        public IEnumerable<Profile> Profiles()
        {
            return _repository;
        }

        public void ActivateProfile(string email, string displayName)
        {
            if (_repository.Any(profile => profile.EmailAddress == email))
                return;

            _repository.Add(new Profile(email, displayName));
        }

        public Profile ProfileFor(string email)
        {
            return _repository.Single(profile => profile.EmailAddress == email);
        }

        public void UpdateDisplayName(string emailAddress, string displayName)
        {
            var profile = _repository.Single(p => p.EmailAddress == emailAddress);

            _repository.Remove(profile);
            _repository.Add(new Profile(emailAddress, displayName));
        }
    }
}