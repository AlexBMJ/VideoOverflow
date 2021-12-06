namespace VideoOverflow.Infrastructure; 

public interface IVideoOverflowContext : IDisposable {
    DbSet<Tag> Tags { get; }
    DbSet<TagSynonym> TagSynonyms { get; }
    DbSet<Category> Categories { get; }
    DbSet<Comment> Comments { get; }
    DbSet<User> Users { get; }
    DbSet<Resource> Resources { get; }
    
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}