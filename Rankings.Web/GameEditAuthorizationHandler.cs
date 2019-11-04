using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rankings.Core.Entities;

namespace Rankings.Web
{
    public class GameEditAuthorizationHandler : AuthorizationHandler<GameEditRequirement, Game>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GameEditRequirement requirement, Game resource)
        {
            if (IsAuthorized(context, resource))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }

        private bool IsAuthorized(AuthorizationHandlerContext context, Game game)
        {
            var user = context.User;

            if (user.IsInRole("Admin"))
                return true;

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