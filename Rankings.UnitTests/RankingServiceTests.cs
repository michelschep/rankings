using System.Collections.Generic;
using System.Linq;
using Moq;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Xunit;

namespace Rankings.UnitTests
{
    public class RankingServiceTests
    {
        private readonly Mock<IRepository> _repository;
        private readonly RankingService _rankingService;

        public RankingServiceTests()
        {
            _repository = new Mock<IRepository>();
            _rankingService = new RankingService(_repository.Object);
        }

        [Fact]
        public void WhenNoGamesTheRankingIsEmpty()
        {
            _repository.Setup(r => r.List<Game>()).Returns(new List<Game>());

            var ranking = _rankingService.Ranking();

            Assert.True(!ranking.Any());
        }
    }
}
