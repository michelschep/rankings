using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rankings.Core.Interfaces;
using Rankings.Core.Specifications;

namespace Rankings.Web
{
    public class GameEditAuthorizationHandler : AuthorizationHandler<GameEditRequirement, string>
    {
        private readonly IGamesProjection _gamesProjection;

        public GameEditAuthorizationHandler(IGamesProjection gamesProjection)
        {
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GameEditRequirement requirement, string resource)
        {
            if (IsAuthorized(context, resource))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }

        private bool IsAuthorized(AuthorizationHandlerContext context, string resource)
        {
            var user = context.User;

            // TODO can we move this to other requirement?
            if (user.IsInRole(Roles.Admin))
                return true;

            var game = _gamesProjection.Item(new SpecificGame(resource));

            if (game == null)
                return false;
            
            if (game.RegistrationDate < DateTime.Now.AddHours(-24))
                return false;

            if (user.Identity.Name == game.Player1.EmailAddress)
                return true;

            if (user.Identity.Name == game.Player2.EmailAddress)
                return true;

            return false;
        }
    }
}