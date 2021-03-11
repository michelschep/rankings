using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rankings.Core.Aggregates.Games;
using Rankings.Core.Interfaces;
using Rankings.Core.Models;
using Rankings.Core.Projections;
using Rankings.Core.Services;
using Rankings.Infrastructure;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;
using Serilog;

namespace Rankings.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Player",
                    policy => policy.RequireRole(Roles.Admin, Roles.Player));
                options.AddPolicy("AdminPolicy",
                    policy => policy.RequireRole(Roles.Admin));
                options.AddPolicy("GameEditPolicy",
                    policy => policy.AddRequirements(new GameEditRequirement()));
                options.AddPolicy("ProfileEditPolicy",
                    policy => policy.AddRequirements(new ProfileEditRequirement()));
            });

            services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews(options =>
                {
                    options.EnableEndpointRouting = false; // TODO new in .net core 3
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            // TODO retrieve from app settings/or admin settings page/object
            
            // TODO PBI needed?
            //services.AddSingleton(Log.Logger);
            
            services.AddTransient<RankingContext>(provider =>
            {
                var factory = new SqLiteRankingContextFactory(Configuration);
                return factory.CreateDbContext1();
            });
            services.AddTransient<IEventStore, EventStore>();
            services.AddSingleton<IRankingsClock, RankingsClock>();
            services.AddSingleton<EloConfiguration, EloConfiguration>(ctx => new EloConfiguration(50, 400, true, 1200, 15));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationHandler, GameEditAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ProfileEditAuthorizationHandler>();
            services.AddSingleton<IEloCalculatorFactory, YearDependentEloCalculatorFactory>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.Configure<RepositoryConfiguration>(Configuration.GetSection("Repository"));

            services.AddTransient(provider =>
            {
                var contextFactory = new SqLiteRankingContextFactory(Configuration);
                var repositoryFactory = new RepositoryFactory(contextFactory);
                return repositoryFactory.Create();

            });
            services.AddTransient<IGamesProjection, GamesProjection>();

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<RegisterSingleGameCommandConsumer>();
                cfg.AddConsumer<SingleGameRegisteredProjectionConsumer>();
                cfg.AddConsumer<SingleGamesProjectionConsumer>();
                
                cfg.UsingInMemory((context, cfg) =>
                {
                    cfg.TransportConcurrencyLimit = 100;

                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
            
            if (env.IsDevelopment() || env.IsEnvironment("Test"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Errors/500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Errors/404";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseMiddleware(typeof(DevelopmentAuthenticationMiddleware));
            }
            else
            {
                app.UseAuthentication();
            }

            app.UseAuthorization();
            app.UseMiddleware(typeof(AuditAuthenticationMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
