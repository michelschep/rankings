using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace Rankings.Web
{
    public class AuditAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditAuthenticationMiddleware> _logger;

        public AuditAuthenticationMiddleware(RequestDelegate next, ILogger<AuditAuthenticationMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof (next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Visitor {username} for page {page}", context.User.Identity.Name, context.Request.GetDisplayUrl()); 
            await _next(context);
        }
    }
}