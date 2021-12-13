using VideoOverflow.Infrastructure.Context;

namespace VideoOverflow.Infrastructure;
public class Program
{
    static void Main(string[] args)
    {
        var configuration = LoadConfiguration();
        var connectionString = configuration.GetConnectionString("VideoOverflow");

        var optionsBuilder = new DbContextOptionsBuilder<VideoOverflowContext>().UseNpgsql(connectionString);
        using var context = new VideoOverflowContext(optionsBuilder.Options);
     
    }

    static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>();

        return builder.Build();
    }
}