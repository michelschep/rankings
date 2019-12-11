using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rankings.Core.Interfaces;
using Rankings.Core.Services;
using Rankings.Infrastructure.Data;
using Rankings.Infrastructure.Data.SqLite;

namespace Rankings.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false; // TODO new in .net core 3
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddSingleton<EloConfiguration, EloConfiguration>(ctx => new EloConfiguration(50, 400, true, 1200));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationHandler, GameEditAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ProfileEditAuthorizationHandler>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<IGamesService, GamesService>((ctx) =>
            {
                var connectionFactory = new SqLiteDatabaseConnectionFactory();
                var sqLiteRankingContextFactory = new SqLiteRankingContextFactory(connectionFactory);
                var repositoryFactory = new RepositoryFactory(sqLiteRankingContextFactory);
                // TODO what if setting is null or empty
                var repository = repositoryFactory.Create(Configuration["Database"]);

                return new GamesService(repository);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Test"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseMiddleware(typeof(DevelopmentAuthenticationMiddleware));
            }
            else
            {
                app.UseAuthentication();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
