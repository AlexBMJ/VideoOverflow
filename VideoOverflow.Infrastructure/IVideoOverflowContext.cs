namespace VideoOverflow.Infrastructure; 

/// <summary>
/// The context for our database
/// </summary>
public interface IVideoOverflowContext : IDisposable {
    DbSet<Tag> Tags { get; }
    DbSet<Word> Words { get; }
    DbSet<TagSynonym> TagSynonyms { get; }
    DbSet<Category> Categories { get; }
    DbSet<Comment> Comments { get; }
    DbSet<User> Users { get; }
    DbSet<Resource> Resources { get; }
    
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}