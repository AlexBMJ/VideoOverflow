namespace VideoOverflow.Infrastructure.Tests;
// https://dev.to/davidkudera/creating-new-postgresql-db-for-every-xunit-test-2h73
public abstract class DatabaseTestCase : IDisposable, IClassFixture<DatabaseTemplateFixture> {
    protected DatabaseTestCase(DatabaseTemplateFixture databaseFixture) {
        var id = Guid.NewGuid().ToString().Replace("-", "");
        var databaseName = $"test_database_{id}";
        using (var tmplConnection = new NpgsqlConnection(databaseFixture.Connection)) {
            tmplConnection.Open();
            using (var cmd = new NpgsqlCommand($"create database {databaseName} with template {databaseFixture.TemplateDatabaseName}", tmplConnection)) {
                cmd.ExecuteNonQuery();
            }
        }
        var connection = $"Server=localhost;Database={databaseName};Port=5002;UserId=postgres;Password=test_db";
        var optionsBuilder = new DbContextOptionsBuilder<VideoOverflowContext>();
        optionsBuilder.UseNpgsql(connection);
        _pgContext = new VideoOverflowContext(optionsBuilder.Options);
    }
    public VideoOverflowContext _pgContext { get; }
    
    public void Dispose() {
        _pgContext.Database.EnsureDeleted();
    }
}