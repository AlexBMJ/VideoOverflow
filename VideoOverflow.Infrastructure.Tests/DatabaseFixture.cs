namespace VideoOverflow.Infrastructure.Tests; 
// https://dev.to/davidkudera/creating-new-postgresql-db-for-every-xunit-test-2h73
public class DatabaseTemplateFixture : IDisposable {
    private readonly DbContext _context;
    
    public DatabaseTemplateFixture() {
        var id = Guid.NewGuid().ToString().Replace("-", "");
        TemplateDatabaseName = $"template_database_{id}";
        Connection = $"Server=localhost;Database={TemplateDatabaseName};Port=5002;UserId=postgres;Password=test_db";
        var optionsBuilder = new DbContextOptionsBuilder<VideoOverflowContext>();
        optionsBuilder.UseNpgsql(Connection);
        _context = new VideoOverflowContext(optionsBuilder.Options);
        _context.Database.EnsureCreated();

        // todo: Insert common data here
        
        _context.Database.CloseConnection();
    }
    public string TemplateDatabaseName { get; }
    public string Connection { get; }

    public void Dispose() {
        _context.Database.EnsureDeleted();
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollectionFixture : ICollectionFixture<DatabaseTemplateFixture> {}