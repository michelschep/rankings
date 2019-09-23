using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Services
{
    public class RankingService : IRankingService
    {
        private readonly IRepository _repository;

        public RankingService(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<Profile> Profiles()
        {
            return _repository.List<Profile>();
        }

        public void ActivateProfile(string email, string displayName)
        {
            if (_repository.List<Profile>().Any(profile => profile.EmailAddress == email))
                return;

            _repository.Add(new Profile(email, displayName));
        }

        public Profile ProfileFor(string email)
        {
            return _repository.List<Profile>().Single(profile => profile.EmailAddress == email);
        }

        public void UpdateDisplayName(string emailAddress, string displayName)
        {
            var profile = _repository.List<Profile>().Single(p => p.EmailAddress == emailAddress);
            profile.DisplayName = displayName;
            //_repository.Delete(profile);
            //_repository.Add(new Profile(emailAddress, displayName));
        }

        public IEnumerable<GameType> GameTypes()
        {
            return _repository.List<GameType>();
        }

        public void CreateGameType(GameType gameType)
        {
            _repository.Add(gameType);
        }

        public IEnumerable<Game> Games()
        {
            return _repository.List<Game>();
        }

        public void RegisterGame(Game game)
        {
            game.RegistrationDate = DateTime.Now;

            _repository.Add(game);
        }

        public void CreateVenue(Venue venue)
        {
            _repository.Add(venue);
        }

        public IEnumerable<Venue> GetVenues()
        {
            return _repository.List<Venue>();
        }
    }
}