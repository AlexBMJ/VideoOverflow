namespace VideoOverflow.Server.Tests;

public class QueryParserTestsSetup
{
    public VideoOverflowContext Context { get; }

    public QueryParserTestsSetup()
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