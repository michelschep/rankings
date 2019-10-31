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
        private readonly decimal _initalElo;
        private readonly int _precision;
        private readonly EloCalculator _eloCalculator;

        public RankingService(IRepository repository, decimal initalElo = 1200, int kfactor = 50, int n = 400, bool withMarginOfVictory = true, int precision = 0)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _initalElo = initalElo;
            _precision = precision;
            _eloCalculator = new EloCalculator(n, kfactor, withMarginOfVictory);
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

        public IEnumerable<Venue> GetVenues()
        {
            return _repository.List<Venue>();
        }

        public Ranking Ranking(string gameType)
        {
            return Ranking(gameType, DateTime.MaxValue);
        }

        public Ranking Ranking(string gameType, DateTime rankingDate)
        {
            // TODO fix loading entities
            var players = Profiles().ToList();
            var gameTypes = GameTypes();
            var venues = GetVenues();

            var games = Games().ToList()
                .Where(game => game.GameType.Code == gameType)
                .OrderBy(game => game.RegistrationDate)
                .ToList();

            var ratings = new Dictionary<Profile, PlayerStats>();
            foreach (var profile in games.SelectMany(game => new List<Profile> {game.Player1, game.Player2}).Distinct())
            {
                ratings.Add(profile, new PlayerStats()
                {
                    NumberOfGames = 0,
                    NumberOfSetWins = 0,
                    NumberOfSets = 0,
                    NumberOfWins = 0,
                    Ranking = _initalElo,
                    History = ""
                });
            }

            foreach (var game in games.Where(game => game.RegistrationDate <= rankingDate))
            {
                // TODO for tafel tennis a 0-0 is not a valid result. For time related games it is possible
                // For now ignore a 0-0
                if (game.Score1 == 0 && game.Score2 == 0)
                    continue;

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var oldRatingPlayer1 = ratings[game.Player1];
                var oldRatingPlayer2 = ratings[game.Player2];

                var player1Delta = CalculateDeltaFirstPlayer(oldRatingPlayer1.Ranking, oldRatingPlayer2.Ranking, game.Score1,
                    game.Score2);

                var newRatingPlayer1 = oldRatingPlayer1.Ranking + player1Delta;
                var newRatingPlayer2 = oldRatingPlayer2.Ranking - player1Delta;

                ratings[game.Player1].Ranking = newRatingPlayer1;
                ratings[game.Player2].Ranking = newRatingPlayer2;

                ratings[game.Player1].NumberOfGames += 1;
                ratings[game.Player2].NumberOfGames += 1;

                ratings[game.Player1].NumberOfWins += game.Score1 > game.Score2 ? 1 : 0;
                ratings[game.Player2].NumberOfWins += game.Score2 > game.Score1 ? 1 : 0;

                if (game.Score1 > game.Score2)
                {
                    ratings[game.Player1].History += "W";
                    ratings[game.Player2].History += "L";
                }

                if (game.Score1 < game.Score2)
                {
                    ratings[game.Player1].History += "L";
                    ratings[game.Player2].History += "W";
                }

                if (game.Score1 == game.Score2)
                {
                    ratings[game.Player1].History += "D";
                    ratings[game.Player2].History += "D";
                }

                ratings[game.Player1].NumberOfSets += game.Score1 + game.Score2;
                ratings[game.Player2].NumberOfSets += game.Score1 + game.Score2;

                ratings[game.Player1].NumberOfSetWins += game.Score1;
                ratings[game.Player2].NumberOfSetWins += game.Score2;
            }

            return new Ranking(ratings, _precision);
        }

        public decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            return _eloCalculator.CalculateDeltaPlayer(ratingPlayer1, ratingPlayer2, gameScore1, gameScore2);
        }
    }
}