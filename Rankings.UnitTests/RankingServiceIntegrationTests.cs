using System.Collections.Generic;
using System.Linq;
using Moq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Xunit;

namespace Rankings.UnitTests
{
    public class RankingServiceTests
    {
        private readonly RankingService _rankingService;

        public RankingServiceTests()
        {
            var rankingContextFactory = new InMemoryRankingContextFactory();
            var repositoryFactory = new RepositoryFactory(rankingContextFactory);
            var repository = repositoryFactory.Create("Test");
            _rankingService = new RankingService(repository);
        }

        [Fact]
        public void WhenNoGamesTheRankingIsEmpty()
        {
            var ranking = _rankingService.Ranking();

            Assert.True(!ranking.Any());
        }

        [Fact]
        public void WhenFirstGameIsADraw()
        {
            var gameType = new GameType
            {
                Code = "tafeltennis",
                DisplayName = "Tafeltenis"
            };
            _rankingService.CreateGameType(gameType);

            var game = new Game()
            {
                GameType = gameType,
                //Player1 = player1
            };
            _rankingService.RegisterGame(game);
            var ranking = _rankingService.Ranking();

            Assert.True(!ranking.Any());
        }
    }
}
