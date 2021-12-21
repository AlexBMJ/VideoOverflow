
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace VideoOverflow.Infrastructure.repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly IVideoOverflowContext _context;

    public ResourceRepository(IVideoOverflowContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ResourceDTO>> GetAll()
    {
        // Give resourceDTO
        return await _context.Resources.Select(c => new ResourceDTO(
            c.Id,
            c.MaterialType,
            c.SiteUrl,
            c.SiteTitle,
            c.Created,
            c.Author,
            c.Language)).ToListAsync();
    }

    public async Task<IEnumerable<ResourceDTO>> GetResources(int category, string query, IEnumerable<TagDTO> tags, int count, int skip)
    {
        return await _context.Resources.
            Where(t => (category == 0 || t.Categories.Any(c=>c.Id == category)) && t.Tags.Any(a=> tags.Select(qt=>qt.Id).Contains(a.Id))).
            OrderByDescending(o=>EF.Functions.TrigramsSimilarity(o.SiteTitle, query)).
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

    public async Task<Status> Update(ResourceUpdateDTO update)
    {
        var entity = await _context.Resources.Where(resource => resource.Id == update.Id).Select(c => c)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return Status.NotFound;
        }

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
        entity.Categories = await GetCategories(update.Categories);
        entity.Tags = await GetTags(update.Tags);
        entity.SiteUrl = update.SiteUrl;
        entity.Created = update.Created;
        entity.SkillLevel = GetSkillLevel(update.LixNumber);
        entity.ContentSource = GetContentSource(update.SiteUrl);
        if (update.Comments != null)
        {
            entity.Comments = await GetComments(update.Comments);
        }

        await _context.SaveChangesAsync();
        return Status.Updated;
    }

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

    private int GetSkillLevel(int lix)
    {
        return lix < 25 ? 1 : lix < 35 ? 2 : lix < 45 ? 3 : lix < 55 ? 4 : 5;
    }
    
    private bool isValidUrl(string url)
    {
        return new Regex(@"(https?:\/\/|www\.)[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)").Match(url).Success;

    }

    private string GetContentSource(string url)
    {
        return new Regex(@"^(?:.*:\/\/)?(?:www\.)?(?<site>[^:\/]*).*$").Match(url).Groups[1].Value;
    }

    private async Task<ICollection<Comment>> GetComments(IEnumerable<string> comments)
    {
        var collectionOfCategories = new Collection<Comment>();
        foreach (var comment in comments)
        {
            var exists = await _context.Comments.FirstOrDefaultAsync(c => c.Content == comment);

            if (exists == null)
            {
                exists = new Comment() {Content = comment};
                await _context.Comments.AddAsync(exists);
                await _context.SaveChangesAsync();
            }

            collectionOfCategories.Add(exists);
        }

        return collectionOfCategories;
    }
}