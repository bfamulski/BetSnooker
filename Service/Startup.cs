using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Helpers;
using BetSnooker.HttpHelper;
using BetSnooker.Repositories;
using BetSnooker.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AuthenticationService = BetSnooker.Services.AuthenticationService;
using IAuthenticationService = BetSnooker.Services.IAuthenticationService;

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
            services.AddTransient<IAdminRepository, AdminRepository>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IBetsRepository, BetsRepository>();
            services.AddTransient<IBetsService, BetsService>();
            services.AddTransient<IScoresRepository, ScoresRepository>();
            services.AddTransient<IScoresService, ScoresService>();
            services.AddTransient<ISnookerFeedService, SnookerFeedService>();
            //services.AddSingleton<IStateService, StateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.ApplicationServices.GetService<IStateService>();
        }
    }
}
