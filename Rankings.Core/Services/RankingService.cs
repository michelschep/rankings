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

            if (game.Player2 == null)
                throw new Exception("Cannot register game because player2 is not specified");

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

        public Ranking Ranking()
        {
            // TODO fix loading entities
            //var players = Profiles().ToList();
            //var gameTypes = GameTypes();
            //var venues = GetVenues();

            var games = Games().ToList()
                .Where(game => game.GameType.Code == "tafeltennis")
                .OrderBy(game => game.RegistrationDate)
                .ToList();

            var ratings = new Dictionary<Profile, PlayerStats>();
            foreach (var profile in games.SelectMany(game => new List<Profile> { game.Player1, game.Player2 }).Distinct())
            {
                ratings.Add(profile, new PlayerStats()
                {
                    NumberOfGames = 0,
                    NumberOfSetWins = 0,
                    NumberOfSets = 0,
                    NumberOfWins = 0,
                    Ranking = 1200,
                    History = ""
                });
            }

            foreach (var game in games)
            {
                // TODO for tafeltennis a 0-0 is not a valid result. For time related games it is possible
                // For now ignore a 0-0
                if (game.Score1 == 0 && game.Score2 == 0)
                    continue;

                // TODO ignore games between the same player. This is a hack to solve the consequences of the issue
                // It should not be possible to enter these games.
                if (game.Player1.EmailAddress == game.Player2.EmailAddress)
                    continue;

                var oldRatingPlayer1 = ratings[game.Player1];
                var oldRatingPlayer2 = ratings[game.Player2];

                var player1Delta = CalculateDeltaFirstPlayer(oldRatingPlayer1.Ranking, oldRatingPlayer2.Ranking, game.Score1, game.Score2);

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

            return new Ranking(ratings);
        }

        public decimal CalculateDeltaFirstPlayer(decimal ratingPlayer1, decimal ratingPlayer2, int gameScore1, int gameScore2)
        {
            var K = 50;

            var expectedOutcome1 = CalculateExpectation(ratingPlayer1, ratingPlayer2);
            decimal actualResult = gameScore1 > gameScore2 ? 1 : 0;

            var winnerEloDiff = gameScore1 > gameScore2
                ? ratingPlayer1 - ratingPlayer2
                : ratingPlayer2 - ratingPlayer1;

            var marginOfVicoryMultiplier = (decimal) Math.Log(Math.Abs(gameScore1 - gameScore2) + 1) *
                                           (2.2m / (winnerEloDiff * 0.001m + 2.2m));

            var outcome1 = (actualResult - expectedOutcome1);
            var player1Delta = K * outcome1 * marginOfVicoryMultiplier;

            return player1Delta;
        }

        public decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            return ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);
        }

        public decimal CalculateExpectation(decimal oldRatingPlayer1, decimal oldRatingPlayer2, int numberOfGames)
        {
            var expectationOneSet = ExpectationOneSet(oldRatingPlayer1, oldRatingPlayer2);

            decimal total = 0;
            for (var index = numberOfGames; index >= 0; --index)
            {
                var other = numberOfGames - index;
                if (index < other)
                    break;

                total += ChangeWinningGameWithSpecifiedResult(index, other, expectationOneSet);
            }

            return total;
        }

        private decimal ChangeWinningGame(int numberOfGames, decimal expectedToWinSet)
        {
            var a = ChangeWinningGameWithSpecifiedResult(3, 0, expectedToWinSet);
            var b = ChangeWinningGameWithSpecifiedResult(2, 1, expectedToWinSet);
            var c = ChangeWinningGameWithSpecifiedResult(1, 2, expectedToWinSet);
            var d = ChangeWinningGameWithSpecifiedResult(0, 3, expectedToWinSet);

            return a + b;
        }

        private static decimal ExpectationOneSet(decimal oldRatingPlayer1, decimal oldRatingPlayer2)
        {
            decimal n = 400;
            decimal x = oldRatingPlayer1 - oldRatingPlayer2;
            decimal exponent = -1 * (x / n);
            decimal expected = (decimal) (1 / (1 + Math.Pow(10, (double) exponent)));

            return expected;
        }

        private decimal ChangeWinningGameWithSpecifiedResult(int gameScore1, int gameScore2, decimal expectedToWinSet)
        {
            var numberOfSets = gameScore1 + gameScore2;

            var changeWinningGameWithSpecifiedResult = (double)Factorial(numberOfSets) / (double)(Factorial(gameScore1) * Factorial(gameScore2))
                                                       * Math.Pow((double)expectedToWinSet, gameScore1)
                                                       * Math.Pow((double)(1 - expectedToWinSet), gameScore2);
            return (decimal) changeWinningGameWithSpecifiedResult;
        }

        private int Factorial(int numberOfSets)
        {
            if (numberOfSets == 0)
                return 1;

            if (numberOfSets == 1)
                return 1;

            return numberOfSets * Factorial(numberOfSets - 1);
        }
    }

    public class Ranking
    {
        private readonly Dictionary<Profile, PlayerStats> _ratings;

        public Ranking(Dictionary<Profile, PlayerStats> ratings)
        {
            _ratings = ratings ?? throw new ArgumentNullException(nameof(ratings));
        }

        public Dictionary<Profile, PlayerStats> OldRatings => _ratings;

        public PlayerStats ForPlayer(string emailAddress)
        {
            return _ratings.Where(pair => string.Equals(pair.Key.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase)).Select(pair => ConvertStats(pair.Value)).Single();
        }

        public IEnumerable<PlayerStats> PlayerStats()
        {
            return _ratings.Values.Select(ConvertStats);
        }

        private static PlayerStats ConvertStats(PlayerStats stats)
        {
            return new PlayerStats
            {
                Ranking = Math.Round(stats.Ranking, 0, MidpointRounding.AwayFromZero),
                NumberOfSets = stats.NumberOfSets,
                History = stats.History,
                NumberOfSetWins = stats.NumberOfSetWins,
                NumberOfWins = stats.NumberOfWins,
                NumberOfGames = stats.NumberOfGames
            };
        }
    }

    public class PlayerStats
    {
        public decimal Ranking { get; set; }
        public int NumberOfGames { get; set; }
        public int NumberOfWins { get; set; }
        public int NumberOfSets { get; set; }
        public int NumberOfSetWins { get; set; }
        public string History { get; set; }
    }
}