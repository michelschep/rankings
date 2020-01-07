using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            _next = next ?? throw new ArgumentNullException(nameof (next));
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public async Task Invoke(HttpContext context)
        {
            var identity = new GenericIdentity("mschep@vitas.nl");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, Roles.Admin),
                new Claim(ClaimTypes.Name, "Name"),
                new Claim(ClaimTypes.Surname, "Surname")
            };
            identity.AddClaims(claims);
            
            context.User = new ClaimsPrincipal(identity);
            
            await _next(context);
        }
    }
}