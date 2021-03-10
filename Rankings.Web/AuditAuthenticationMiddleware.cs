using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Rankings.Core.Interfaces;

namespace Rankings.Web
{
    public class AuditAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditAuthenticationMiddleware> _logger;
        private readonly IGamesProjection _gamesProjection;

        public AuditAuthenticationMiddleware(RequestDelegate next, ILogger<AuditAuthenticationMiddleware> logger, IGamesProjection gamesProjection)
        {
            _next = next ?? throw new ArgumentNullException(nameof (next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gamesProjection = gamesProjection ?? throw new ArgumentNullException(nameof(gamesProjection));
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Visitor {username} for page {page}", context.User.Identity.Name, context.Request.GetDisplayUrl());

            try
            {
                var email = context.User.FindFirst(ClaimTypes.Name).Value;
                var name = context.User.FindFirst(ClaimTypes.Surname).Value;
                _gamesProjection.ActivateProfile(email, name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Cannot activate user {context.User.Identity.Name}", ex);
            }

            await _next(context);
        }
    }
}