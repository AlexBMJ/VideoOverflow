namespace VideoOverflow.Infrastructure.Tests;

public class RepositoryTestsSetup
{
    protected readonly VideoOverflowContext _context;
    protected readonly DateTime Created = new (2021,11,30, 13,11,11);

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