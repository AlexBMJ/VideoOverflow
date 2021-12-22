
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Castle.Core.Internal;

namespace VideoOverflow.Infrastructure.repositories;

/// <summary>
/// The repository for the user relation
/// </summary>
public class ResourceRepository : IResourceRepository
{
    private readonly IVideoOverflowContext _context;

    /// <summary>
    /// Initialize the repository with a given context
    /// </summary>
    /// <param name="context">Context for a DB</param>
    public ResourceRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all resources from the resource relation in the DB
    /// </summary>
    /// <returns>A collection of all resources</returns>
    public async Task<IEnumerable<ResourceDetailsDTO>> GetAll()
    {
        // Give resourceDTO
        return await _context.Resources.Select(c => new ResourceDetailsDTO() {
            Id = c.Id,
            MaterialType = c.MaterialType,
            Author = c.Author,
            SiteTitle = c.SiteTitle,
            ContentSource = c.ContentSource,
            SiteUrl = c.SiteUrl,
            Created = c.Created,
            Language = c.Language,
            LixNumber = c.LixNumber,
            SkillLevel = c.SkillLevel,
            Categories = c.Categories.Select(category => category.Name).ToList(),
            Comments = c.Comments.Select(comment => comment.Content).ToList(),
            Tags = c.Tags.Select(t => t.Name).ToList()
        }).ToListAsync();
    }
    
    /// <summary>
    /// Gets all resources in a specific category based on a query from the user which has some specified tags
    /// and only returns a specified amount of resources
    /// </summary>
    /// <param name="category">The id of the category of the resources to get</param>
    /// <param name="query">The query from the user to parse</param>
    /// <param name="tags">All the tags which the resources needs to have</param>
    /// <param name="count">Amount of resources to return</param>
    /// <param name="skip">Number of elements to skip, this is related to the page from the controller</param>
    /// <returns>List with "count" number of resources with the given tags, sorted by similarity to title and then by id, skipping the first number of "skips"</returns>
    public async Task<IEnumerable<ResourceDTO>> GetResources(int category, string query, IEnumerable<TagDTO> tags, int count, int skip)
    {
        return await _context.Resources.
            Where(t => (category == 0 || t.Categories.Any(c=>c.Id == category)) && (tags.IsNullOrEmpty() || t.Tags.Any(a=> tags.Select(qt=>qt.Id).Contains(a.Id)))).
            OrderByDescending(o=>EF.Functions.TrigramsSimilarity(o.SiteTitle, query)).
            ThenBy(res=>res.Id).
            Skip(skip).
            Take(count).
            Select(r => new ResourceDTO(
            r.Id,
            r.MaterialType,
            r.SiteUrl,
            r.SiteTitle,
            r.Created,
            r.Author,
            r.Language)).ToListAsync();
    }

    /// <summary>
    /// Gets a resource based on it's id
    /// </summary>
    /// <param name="resourceId">The id of the resource to get</param>
    /// <returns>The resource with the specific id</returns>
    public async Task<Option<ResourceDetailsDTO>> Get(int resourceId)
    {
        return await _context.Resources.Where(resource => resource.Id == resourceId).Select(c => 
                new ResourceDetailsDTO()
                {
                    Id = c.Id,
                    MaterialType = c.MaterialType,
                    Author = c.Author,
                    SiteTitle = c.SiteTitle,
                    ContentSource = c.ContentSource,
                    SiteUrl = c.SiteUrl,
                    Created = c.Created,
                    Language = c.Language,
                    LixNumber = c.LixNumber,
                    SkillLevel = c.SkillLevel,
                    Categories = c.Categories.Select(category => category.Name).ToList(),
                    Comments = c.Comments.Select(comment => comment.Content).ToList(),
                    Tags = c.Tags.Select(t => t.Name).ToList()
                })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Pushes a resource to the resource relation in the DB
    /// </summary>
    /// <param name="create">The resource to push to the DB</param>
    /// <returns>The status of the push</returns>

    public async Task<Status> Push(ResourceCreateDTO create) {
        if (!isValidUrl(create.SiteUrl)) return Status.BadRequest;
        
        var resource = new Resource
        {
            Author = create.Author ?? "Unknown",
            Created = create.Created,
            MaterialType = create.MaterialType,
            Language = create.Language,
            LixNumber = create.LixNumber < 0 ? 0 : create.LixNumber,
            ContentSource = GetContentSource(create.SiteUrl),
            SiteTitle = create.SiteTitle,
            SiteUrl = create.SiteUrl,
            Comments = new Collection<Comment>(),
            Tags = await GetTags(create.Tags),
            SkillLevel = GetSkillLevel(create.LixNumber),
            Categories = await GetCategories(create.Categories)
        };

        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();

        return Status.Created;
    }

    /// <summary>
    /// Updates a resource in the DB
    /// </summary>
    /// <param name="update">The updated resource</param>
    /// <returns>The status of the update</returns>
    public async Task<Status> Update(ResourceUpdateDTO update)
    {
        var entity = await _context.Resources.FirstOrDefaultAsync(resource => resource.Id == update.Id);

        if (entity == null)
        {
            return Status.NotFound;
        }

        if (!isValidUrl(update.SiteUrl)) return Status.BadRequest;
        
        if (update.Author == null)
        {
            entity.Author = "Unknown";
        }
        else
        {
            entity.Author = update.Author;
        }

        entity.LixNumber = update.LixNumber;
        entity.Language = update.Language;
        entity.MaterialType = update.MaterialType;
        entity.SiteTitle = update.SiteTitle;
        entity.SiteUrl = update.SiteUrl;
        entity.Created = update.Created;
        entity.SkillLevel = GetSkillLevel(update.LixNumber);
        entity.ContentSource = GetContentSource(update.SiteUrl);

        await _context.SaveChangesAsync();
        return Status.Updated;
    }

    /// <summary>
    /// Deletes a resource from the DB based on it's id
    /// </summary>
    /// <param name="resourceId">The id of the resource to delete</param>
    /// <returns>The status of the delete</returns>
    public async Task<Status> Delete(int resourceId)
    {
        var resource = await _context.Resources.FindAsync(resourceId);

        if (resource == null)
        {
            return Status.NotFound;
        }
        
        await foreach (var comment in _context.Comments)
        {
            if (comment.AttachedToResource == resourceId)
            {
                _context.Comments.Remove(comment);
            }
        }
        
        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();
        
        return Status.Deleted;
    }

    /// <summary>
    /// Gets all tags based on a collection of tag names
    /// </summary>
    /// <param name="tags">A collection of tag names</param>
    /// <returns>A collection of tags with the tag names</returns>
    private async Task<ICollection<Tag>> GetTags(IEnumerable<string> tags)
    {
        var collectionOfTags = new Collection<Tag>();
        foreach (var tag in tags)
        {
            var existsTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag);

            if (existsTag == null)
            {
                existsTag = new Tag() {Name = tag, TagSynonyms = new Collection<TagSynonym>()};
                await _context.Tags.AddAsync(existsTag);
                await _context.SaveChangesAsync();
            }

            collectionOfTags.Add(existsTag);
        }

        return collectionOfTags;
    }

    /// <summary>
    /// Gets categories based on a collection of category names
    /// </summary>
    /// <param name="categories">A collection of category names</param>
    /// <returns>A collection of categories with the category names</returns>
    private async Task<ICollection<Category>> GetCategories(IEnumerable<string> categories)
    {
        var collectionOfCategories = new Collection<Category>();
        foreach (var category in categories)
        {
            var exists = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category);

            if (exists == null)
            {
                exists = new Category() {Name = category};
                await _context.Categories.AddAsync(exists);
                await _context.SaveChangesAsync();
            }

            collectionOfCategories.Add(exists);
        }

        return collectionOfCategories;
    }

    /// <summary>
    /// Gets the skill level of a resource based on it's lix
    /// </summary>
    /// <param name="lix">The lix of a resource</param>
    /// <returns>The skill level of the lix</returns>
    private int GetSkillLevel(int lix)
    {
        return lix < 25 ? 1 : lix < 35 ? 2 : lix < 45 ? 3 : lix < 55 ? 4 : 5;
    }
    
    /// <summary>
    /// Checks if a url is valid
    /// </summary>
    /// <param name="url">The url to check</param>
    /// <returns>Whether the url is valid or not</returns>
    private bool isValidUrl(string url)
    {
        return new Regex(@"(https?:\/\/|www\.)[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)").Match(url).Success;

    }

    /// <summary>
    /// Gets the content source from a url
    /// </summary>
    /// <param name="url">The url to extract a content source from</param>
    /// <returns>The content source of the url</returns>
    private string GetContentSource(string url)
    {
        return new Regex(@"^(?:.*:\/\/)?(?:www\.)?(?<site>[^:\/]*).*$").Match(url).Groups[1].Value;
    }
    

    /// <summary>
    /// Gets all comments based on a collection of comments' content
    /// </summary>
    /// <param name="comments">A collection of comments' content</param>
    /// <returns>A collection of comments with the specified content</returns>
    private async Task<ICollection<Comment>> GetComments(IEnumerable<string> comments)
    {
        var collectionOfComments = new Collection<Comment>();
        foreach (var comment in comments)
        {
            var exists = await _context.Comments.FirstOrDefaultAsync(c => c.Content == comment);

            if (exists == null)
            {
                exists = new Comment() {Content = comment};
                await _context.Comments.AddAsync(exists);
                await _context.SaveChangesAsync();
            }

            collectionOfComments.Add(exists);
        }

        return collectionOfComments;
    }
}