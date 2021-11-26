using Microsoft.EntityFrameworkCore;
using VideoOverflow.Infrastructure.Entities;

namespace VideoOverflow.Infrastructure; 

public interface IVideoOverflowContext : IDisposable {
    DbSet<Tag> Tags { get; }
    DbSet<TagSynonym> TagSynonyms { get; }
    DbSet<Category> Categories { get; }
    DbSet<Comment> Comments { get; }
    DbSet<User> Users { get; }
    DbSet<Resource> Resources { get; }
}