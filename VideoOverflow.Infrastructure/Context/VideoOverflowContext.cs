namespace VideoOverflow.Infrastructure.Context;

/// <summary>
/// The context for our database
/// </summary>
public class VideoOverflowContext : DbContext, IVideoOverflowContext
{
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TagSynonym> TagSynonyms => Set<TagSynonym>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Resource> Resources => Set<Resource>();


    public VideoOverflowContext(DbContextOptions<VideoOverflowContext> options) : base(options)
    {
    }

    /// <summary>
    /// Creates the different relationships and attributes in our relations in the database
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Many to Many
        modelBuilder.Entity<Resource>()
            .HasMany(resource => resource.Tags)
            .WithMany(t => t.Resources);

        modelBuilder.Entity<Resource>()
            .HasMany(resource => resource.Categories)
            .WithMany(category => category.Resources);

        modelBuilder.Entity<Tag>()
            .HasMany(tag => tag.TagSynonyms)
            .WithMany(tagsynonym => tagsynonym.Tags);
        
        modelBuilder.Entity<Tag>()
            .HasIndex(u => u.Name)
            .IsUnique();
        
        modelBuilder.HasPostgresExtension("pg_trgm");

        // One to Many
        modelBuilder.Entity<Resource>()
            .HasMany(resource => resource.Comments);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Comments);
        
        // Convert Enum to string
        
        modelBuilder
            .Entity<Resource>()
            .Property(e => e.MaterialType)
            .HasConversion(new EnumToStringConverter<ResourceType>());
    }
}