namespace VideoOverflow.Infrastructure.Tests;

public class RepositoryTestsSetup
{
    public VideoOverflowContext Context { get; }

    public RepositoryTestsSetup()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<VideoOverflowContext>();
        builder.UseSqlite(connection);
        var context = new VideoOverflowContext(builder.Options);
        context.Database.EnsureCreated();

        context.SaveChanges();

        Context = context;
    }
}