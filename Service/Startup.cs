using System;
using BetSnooker.Helpers;
using BetSnooker.HttpHelper;
using BetSnooker.Repositories;
using BetSnooker.Repositories.Interfaces;
using BetSnooker.Services;
using BetSnooker.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthenticationService = BetSnooker.Services.AuthenticationService;
using IAuthenticationService = BetSnooker.Services.Interfaces.IAuthenticationService;

namespace BetSnooker
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
            services.AddCors();
            services.AddControllers();

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            // configure DI for application services
            services.AddDbContext<InMemoryDbContext>(options => options.UseInMemoryDatabase(databaseName: "BetSnooker"));

            services.AddTransient<IAsyncRestClient, AsyncRestClient>();
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IBetsRepository, BetsRepository>();
            services.AddTransient<IBetsService, BetsService>();
            services.AddTransient<ISnookerFeedService, SnookerFeedService>();
            services.AddTransient<ISnookerApiService, SnookerApiService>();

            (int eventId, int startRound, string snookerApiUrl) = GetConfigurationItems();
            services.AddSingleton<IConfigurationService>(new ConfigurationService(eventId, startRound, snookerApiUrl));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private (int eventId, int startRound, string snookerApiUrl) GetConfigurationItems()
        {
            var eventIdConfig = Configuration["EventID"];
            var startRoundConfig = Configuration["StartRound"];
            if (string.IsNullOrEmpty(eventIdConfig) || string.IsNullOrEmpty(startRoundConfig))
            {
                throw new ApplicationException("EventID and/or StartRound configuration variable is not set");
            }

            if (!int.TryParse(eventIdConfig, out int eventId) || !int.TryParse(startRoundConfig, out int startRound))
            {
                throw new ApplicationException("EventID and/or StartRound configuration variable is invalid");
            }

            var snookerApiUrl = Configuration["SnookerApiUrl"];
            if (string.IsNullOrEmpty(snookerApiUrl))
            {
                throw new ApplicationException("SnookerApiUrl configuration variable is not set");
            }

            return (eventId, startRound, snookerApiUrl);
        }
    }
}
