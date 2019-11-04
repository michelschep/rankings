using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Rankings.Core.Entities;
using Rankings.Web.Models;

namespace Rankings.Web
{
    public class ProfileEditAuthorizationHandler : AuthorizationHandler<ProfileEditRequirement, ProfileViewModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileEditRequirement requirement, ProfileViewModel resource)
        {
            if (IsAuthorized(context, resource))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }

        private bool IsAuthorized(AuthorizationHandlerContext context, ProfileViewModel profile)
        {
            var user = context.User;

            // TODO put roles in list 
            if (user.IsInRole("Admin"))
                return true;

            if (user.Identity.Name == profile.EmailAddress)
                return true;

            return false;
        }
    }
}