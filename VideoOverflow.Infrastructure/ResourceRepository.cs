using System.Collections.ObjectModel;

namespace VideoOverflow.Infrastructure;

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
        throw new NotImplementedException();
    }
    
    public async Task<ResourceDetailsDTO?> Get(int id)
    {
        // Give ResourceDetailsDTO 
        throw new NotImplementedException();
    }

    public async Task<ResourceDTO> Push(ResourceCreateDTO create)
    {
        var resource = new Resource()
        {
            Author = create.Author,
            Created = create.Created,
            MaterialType = create.MaterialType,
            Language = create.Language,
            LixNumber = create.LixNumber,
            SiteTitle = create.SiteTitle,
            SiteUrl = create.SiteUrl,
            ContentSource = create.ContentSource,
            Comments = new Collection<Comment>(),
            Tags = await GetTags(create.Tags),
            Categories = await GetCategories(create.Categories)

        };
        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();


        return new ResourceDTO(resource.Id,
            resource.MaterialType,
            resource.SiteUrl,
            resource.SiteTitle,
            resource.Author,
            resource.Language,
            resource.Tags.Select(c => c.Name).ToList(),
            resource.Categories.Select(c => c.Name).ToList(),
            new List<string>()
        );
    }

    public async Task<Status> Update(int id, ResourceUpdateDTO update)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Tag>> GetTags(IEnumerable<string> tags)
    {
        var collectionOfTags = new Collection<Tag>();
        foreach (var tag in tags)
        {
            var exists = await _context.Tags.FirstOrDefaultAsync(c => c.Name == tag);

            if (exists == null)
            {
                exists = new Tag() {Name = tag};
                await _context.Tags.AddAsync(exists);
                await _context.SaveChangesAsync();
            }
            collectionOfTags.Add(exists);
        }
        return collectionOfTags;
    }
    
    public async Task<ICollection<Category>> GetCategories(IEnumerable<string> categories)
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
}