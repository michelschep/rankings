using System;
using System.Collections.Generic;
using System.Linq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;

namespace Rankings.Core.Services
{
    public class GamesService : IGamesService
    {
        private readonly IRepository _repository;
       
        public GamesService(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
          
        }

        public IEnumerable<Profile> Profiles()
        {
            return _repository.List<Profile>();
        }

        public void ActivateProfile(string email, string displayName)
        {
            if (_repository.List<Profile>().Any(profile => profile.EmailAddress.ToLower() == email.ToLower()))
                return;

            _repository.Add(new Profile
            {
                EmailAddress = email,
                DisplayName = displayName
            });
        }

        public Profile ProfileFor(string email)
        {
            return _repository.List<Profile>().SingleOrDefault(profile =>
                string.Equals(profile.EmailAddress, email, StringComparison.CurrentCultureIgnoreCase));
        }

        public void UpdateDisplayName(string emailAddress, string displayName)
        {
            var profile = _repository.List<Profile>().Single(p => p.EmailAddress == emailAddress);
            profile.DisplayName = displayName;
            _repository.Update(profile);
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
            if (game.Player1 == null)
                throw new Exception("Cannot register game because player1 is not specified");

            if (_repository.GetById<Profile>(game.Player1.Id) == null)
                throw new Exception("Cannot register game because player1 is not registered");

            if (game.Player2 == null)
                throw new Exception("Cannot register game because player2 is not specified");

            if (_repository.GetById<Profile>(game.Player2.Id) == null)
                throw new Exception("Cannot register game because player2 is not registered");

            game.RegistrationDate = DateTime.Now;

            _repository.Add(game);
        }

        public void CreateVenue(Venue venue)
        {
            _repository.Add(venue);
        }

        public void DeleteGame(int Id)
        {
            // TODO use getbyid to delete
            var entity = _repository.GetById<Game>(Id);
            _repository.Delete(entity);
        }

        public void CreateProfile(Profile profile)
        {
            // TODO give feedback to client
            if (_repository.List<Profile>().Any(profile1 => profile1.EmailAddress == profile.EmailAddress))
                return;

            _repository.Add(profile);
        }

        public void Save(Game entity)
        {
            _repository.Update(entity);
        }

        public void Save(Venue entity)
        {
            _repository.Update(entity);
        }

        public IEnumerable<Venue> GetVenues()
        {
            return _repository.List<Venue>();
        }
    }
}