using VideoOverflow.Infrastructure.Context;

namespace VideoOverflow.Server.Integration.Tests;

public class VideoOverflowWebApplicationFactory : WebApplicationFactory<Program>
{
    /* This method has been taken from: https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Server.Integration.Tests/CustomWebApplicationFactory.cs */
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VideoOverflowContext>));

            if (dbContext != null)
            {
                services.Remove(dbContext);
            }

            /* Overriding policies and adding Test Scheme defined in TestAuthHandler */
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Test")
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            
            var connection = new SqliteConnection("Filename=:memory:");

            services.AddDbContext<VideoOverflowContext>(options =>
            {
                options.UseSqlite(connection);
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<VideoOverflowContext>();
            appContext.Database.OpenConnection();
            appContext.Database.EnsureCreated();
        });
        
        builder.UseEnvironment("Integration");

        builder.Build();
        
        return base.CreateHost(builder);
    }
}
