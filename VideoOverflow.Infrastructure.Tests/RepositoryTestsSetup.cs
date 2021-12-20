using VideoOverflow.Infrastructure.Context;

namespace VideoOverflow.Infrastructure.Tests;

/// <summary>
/// Setup for our repositoryTests
/// </summary>
public class RepositoryTestsSetup
{
    protected readonly VideoOverflowContext _context;
    protected readonly DateTime Created = DateTime.Parse("2020-09-29");

    /// <summary>
    /// Makes a connection to an in memory DB,
    /// and saves the context for it
    /// </summary>
    protected RepositoryTestsSetup()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<VideoOverflowContext>();
        builder.UseSqlite(connection);
        var context = new VideoOverflowContext(builder.Options);
        context.Database.EnsureCreated();

        context.SaveChanges();

        _context = context;
    }
}