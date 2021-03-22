using fmx_cah_host.Hubs;
using fmx_cah_host.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fmx_cah_host
{
    public class Startup
    {
        // Global settings used for JWT Authentication
        private static string JwtSecret = Environment.GetEnvironmentVariable("jwt_secret");
        public const string JwtAuthScheme = "JWT";
        public const string JwtIssuer = "ForceMX";
        public const string JwtAudience = "CAH";
        public static readonly SymmetricSecurityKey JwtSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));


        public Startup(IConfiguration configuration)
        {
            // Dont complete startup if there is no JWT key
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("jwt_secret")))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Missing required environmental variable for JWT authentication: jwt_secret");
                Console.WriteLine("Please setup a secure jwt_secret variable of sufficent length.");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup CORS policies
            services.AddCors(options =>
            {
                options.AddPolicy("Development", builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin => true);
                });

                options.AddPolicy("Production", builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()   
                    .AllowCredentials()
                    .WithOrigins("https://www.forcemx.com"); // This is where we host the game server
                });
            });

            // Add JWT setup
            services.AddAuthentication(JwtAuthScheme).AddJwtBearer(JwtAuthScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateActor = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = JwtSigningKey,
                    ValidIssuer = JwtIssuer,
                    ValidAudience = JwtAudience
                };

                // Add tokens that are passed in the URL to the Token header
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Query.ContainsKey("access_token"))
                            ctx.Token = ctx.Request.Query["access_token"];
                        return Task.CompletedTask;
                    }
                };
            });


            // Tell SignalR how to obtain the users ID.
            services.AddSingleton<IUserIdProvider, UserIdProviderService>();
            services.AddSignalR(options =>
            {
                // Timeout handshakes that are taking too long
                options.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });

            // Game Services
            services.AddSingleton<GameContainerService>();
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<CardDbService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("Development");
            }
            else
            {
                app.UseCors("Production");
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("game-hub");
            });
        }
    }
}
