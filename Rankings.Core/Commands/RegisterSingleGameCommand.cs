using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Microsoft.Extensions.Logging;
using Rankings.Core.Entities;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;

namespace Rankings.Core.Commands
{
    public class RegisterSingleGameCommand
    {
        public Game Game { get; }

        public RegisterSingleGameCommand(Game game)
        {
            Game = game;
        }
    }

    public class RegisterSingleGameConsumer : IConsumer<RegisterSingleGameCommand>
    {
        private readonly IGamesService _gamesService;
        private readonly ILogger<RegisterSingleGameConsumer> _logger;

        public RegisterSingleGameConsumer()
        {
        }

        public RegisterSingleGameConsumer(IGamesService gamesService, ILogger<RegisterSingleGameConsumer> logger)
        {
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<RegisterSingleGameCommand> context)
        {
            _logger.LogInformation("Consume message {message}", context.Message);
            _gamesService.RegisterGame(context.Message.Game);

            // await context.Publish<OrderSubmitted>(new
            // {
            //     context.Message.OrderId
            // });
        }
    }
}
