using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Rankings.Web
{
    public class DevelopmentAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public DevelopmentAuthenticationMiddleware(RequestDelegate next)
        {
            this._next = next ?? throw new ArgumentNullException(nameof (next));
        }

        public async Task Invoke(HttpContext context)
        {
            var identity = new GenericIdentity("admin@domain.nl");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, Roles.Admin),
                new Claim(ClaimTypes.Name, "Name"),
                new Claim(ClaimTypes.Surname, "Surname")
            };
            identity.AddClaims(claims);
            
            context.User = new ClaimsPrincipal(identity);
            
            await this._next(context);
        }
    }

    public class Roles
    {
        public const string Admin = "Admin";
        public const string Player = "Player";
    }
}