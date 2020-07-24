using System;
using System.IO;
using System.Reflection;
using BetSnooker.Configuration;
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
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
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
            services.AddLogging();
            services.AddApplicationInsightsTelemetry();

            services.AddCors();
            services.AddControllers();

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo { Title = "BetSnooker API", Version = "v1", Description = "BetSnooker REST API", Contact = new OpenApiContact
                    {
                        Name = "Bogus�aw Famulski",
                        Email = "boguslaw.famulski@gmail.com"
                    }});

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            // configure DI for application services
            services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: "BetSnooker"));
            //services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BetSnookerDatabase")));

            services.AddSingleton<ISettings, Settings>();

            services.AddTransient<IAsyncRestClient, AsyncRestClient>();
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IBetsRepository, BetsRepository>();
            services.AddTransient<IBetsService, BetsService>();
            services.AddTransient<ISnookerFeedService, SnookerFeedService>();
            services.AddTransient<ISnookerApiService, SnookerApiService>();
            services.AddSingleton<ISnookerHubService, SnookerHubService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // global cors policy
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", "BetSnooker API v1"));

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.LogInformation("Service started successfully");
        }
    }
}
