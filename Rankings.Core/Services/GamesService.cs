using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.SharedKernel;
using Rankings.Core.Specifications;

namespace Rankings.Core.Services
{
    public class GamesService : IGamesService
    {
        private readonly IRepository _repository;
       
        public GamesService(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<T> List<T>(ISpecification<T> specification) where T : BaseEntity
        {
            return _repository.List(specification);
        }

        public T Item<T>(ISpecification<T> specification) where T : BaseEntity
        {
            return List(specification).SingleOrDefault();
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

        public void CreateProfile(Profile profile)
        {
            if (List(new SpecificProfile(profile.EmailAddress)).Any())
                return;

            _repository.Add(profile);
        }

        public void UpdateDisplayName(string emailAddress, string displayName)
        {
            var profile = _repository.List<Profile>().Single(p => p.EmailAddress == emailAddress);
            profile.DisplayName = displayName;
            _repository.Update(profile);
        }

        public void CreateVenue(Venue venue)
        {
            _repository.Add(venue);
        }

        public void Save(Venue entity)
        {
            _repository.Update(entity);
        }

        public void CreateGameType(GameType gameType)
        {
            _repository.Add(gameType);
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

        public void Save(Game entity)
        {
            _repository.Update(entity);
        }

        public void DeleteGame(int id)
        {
            _repository.Delete(_repository.GetById<Game>(id));
        }
    }
}