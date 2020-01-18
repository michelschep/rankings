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
        private readonly IGamesService _gamesService;

        public AuditAuthenticationMiddleware(RequestDelegate next, ILogger<AuditAuthenticationMiddleware> logger, IGamesService gamesService)
        {
            _next = next ?? throw new ArgumentNullException(nameof (next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gamesService = gamesService ?? throw new ArgumentNullException(nameof(gamesService));
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Visitor {username} for page {page}", context.User.Identity.Name, context.Request.GetDisplayUrl());

            try
            {
                var email = context.User.FindFirst(ClaimTypes.Name).Value;
                var name = context.User.FindFirst(ClaimTypes.Surname).Value;
                _gamesService.ActivateProfile(email, name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Cannot activate user {context.User.Identity.Name}");
            }

            await _next(context);
        }
    }
}