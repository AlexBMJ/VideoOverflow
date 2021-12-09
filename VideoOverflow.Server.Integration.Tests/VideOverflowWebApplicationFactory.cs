using System.Formats.Asn1;
using Microsoft.Extensions.DependencyInjection;


namespace VideoOverflow.Server.Integration.Tests;

public class VideoOverflowWebApplicationFactory : WebApplicationFactory<Program>
{
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
                options.UseNpgsql(connection);
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<VideoOverflowContext>();
            appContext.Database.OpenConnection();
            appContext.Database.EnsureCreated();

        });
        
        builder.UseEnvironment("Integration");

        builder.Build().FillDatabase();
            
        return base.CreateHost(builder);
    }
    
}