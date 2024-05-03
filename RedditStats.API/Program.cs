using RedditStats.Models;
using RedditStats.Services;

namespace RedditStats.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Configure the application services and dependencies
            ConfigureAppServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            ConfigurePipeline(app, app.Environment);

            app.Run();
        }

        private static void ConfigureAppServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configure logging
            services.AddLogging(configure => configure.AddConsole());

            // Register services and dependencies
            services.AddHttpClient<IRedditService, RedditService>();
            services.AddSingleton<IRateLimiter, RateLimiter>();

            // Configure options from appsettings.json
            services.Configure<RedditApiSettings>(configuration.GetSection("RedditApi"));

            // Add controllers and other services
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void ConfigurePipeline(WebApplication app, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
