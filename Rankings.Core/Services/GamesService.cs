using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.SharedKernel;
using Rankings.Core.Specifications;
using static System.String;

namespace Rankings.Core.Services
{
    public class GamesService : IGamesService
    {
        private readonly IRepository _repository;
        private readonly IRankingsClock _clock;

        public GamesService(IRepository repository, IRankingsClock rankingsClock)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _clock = rankingsClock;
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
                Identifier = Guid.NewGuid().ToString(),
                EmailAddress = email,
                DisplayName = displayName
            });
        }

        public void CreateProfile(Profile profile)
        {
            if (List(new SpecificProfile(profile.EmailAddress)).Any())
                return;

            if (IsNullOrEmpty(profile.EmailAddress))
                throw new Exception("New profile should have valid email address");

            if (IsNullOrEmpty(profile.DisplayName))
                throw new Exception("New profile should have valid display name");

            profile.IsActive = true;
            _repository.Add(profile);
        }

        public void DeactivateProfile(int id)
        {
            var profile = Item(new SpecificProfile(id));
            profile.IsActive = !profile.IsActive;
            _repository.Update(profile);
        }

        public void UpdateDisplayName(string emailAddress, string displayName)
        {
            var profile = Item(new SpecificProfile(emailAddress));
            profile.DisplayName = displayName;
            _repository.Update(profile);
        }

        public void CreateVenue(Venue venue)
        {
            if (Item(new NamedVenue(venue.DisplayName)) != null)
            {
                return;
            }
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
            if (game.GameType == null)
                throw new Exception("Cannot register game because game type is not specified");

            if (game.Player1 == null)
                throw new Exception("Cannot register game because player1 is not specified");

            if (_repository.GetById<Profile>(game.Player1.Id) == null)
                throw new Exception($"Cannot register game because player1 [{game.Player1.DisplayName}] is not registered");

            if (game.Player2 == null)
                throw new Exception("Cannot register game because player2 is not specified");

            if (_repository.GetById<Profile>(game.Player2.Id) == null)
                throw new Exception("Cannot register game because player2 is not registered");

            game.RegistrationDate = _clock.Now();
            game.Identifier = Guid.NewGuid().ToString();

            _repository.Add(game);
        }

        public void RegisterDoubleGame(DoubleGame game)
        {
            game.RegistrationDate = _clock.Now();
            game.Identifier = Guid.NewGuid().ToString();

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

        public bool IsDisplayNameUnique(string displayName)
        {
            return !List(new Profiles(displayName)).ToList().Any();
        }
    }
}