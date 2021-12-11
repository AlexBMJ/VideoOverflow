﻿using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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
        return await _context.Resources.Select(c => new ResourceDTO(
            c.Id,
            c.MaterialType,
            c.SiteUrl,
            c.ContentSource,
            c.SiteTitle,
            c.Author,
            c.Language,
            c.Tags.Select(tag => tag.Name).ToList(),
            c.Categories.Select(category => category.Name).ToList(),
            c.Comments.Select(comment => comment.Content).ToList())).ToListAsync();
    }

    public Task<IEnumerable<ResourceDTO>> GetByTags() {
        throw new NotImplementedException();
    }

    public async Task<ResourceDetailsDTO?> Get(int resourceId)
    {
        var entity = await _context.Resources.Where(resource => resource.Id == resourceId).Select(c => c)
            .FirstOrDefaultAsync();
        // Give ResourceDetailsDTO 
        if (entity == null)
        {
            return null;
        }

        return new ResourceDetailsDTO()
        {
            Id = entity.Id,
            MaterialType = entity.MaterialType,
            Author = entity.Author,
            SiteTitle = entity.SiteTitle,
            ContentSource = entity.ContentSource,
            SiteUrl = entity.SiteUrl,
            Created = entity.Created,
            Language = entity.Language,
            LixNumber = entity.LixNumber,
            SkillLevel = entity.SkillLevel,
            Categories = entity.Categories.Select(c => c.Name).ToList(),
            Comments = entity.Comments == null ? new List<string>() : entity.Comments.Select(c => c.Content).ToList(),
            Tags = entity.Tags.Select(c => c.Name).ToList()
        };
    }

    public async Task<ResourceDTO> Push(ResourceCreateDTO create)
    {
        var contentSource = GetContentSource(create.SiteUrl);
        var resource = new Resource()
        {
            Author = create.Author,
            Created = create.Created,
            MaterialType = create.MaterialType,
            Language = create.Language,
            LixNumber = create.LixNumber,
            ContentSource = contentSource,
            SiteTitle = create.SiteTitle,
            SiteUrl = create.SiteUrl,
            Comments = new Collection<Comment>(),
            Tags = await GetTags(create.Tags),
            SkillLevel = GetSkillLevel(create.LixNumber),
            Categories = await GetCategories(create.Categories)
        };


        if (resource.Author == null)
        {
            resource.Author = "Unknown";
        }

        await _context.Resources.AddAsync(resource);
        await _context.SaveChangesAsync();

        return new ResourceDTO(resource.Id,
            resource.MaterialType,
            resource.SiteUrl,
            resource.ContentSource,
            resource.SiteTitle,
            resource.Author,
            resource.Language,
            resource.Tags.Select(c => c.Name).ToList(),
            resource.Categories.Select(c => c.Name).ToList(),
            new List<string>()
        );
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

        await _context.SaveChangesAsync();
        return Status.Updated;
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

    private int GetSkillLevel(int lix)
    {
        if (lix < 25)
        {
            return 1;
        }

        if (lix < 35)
        {
            return 2;
        }

        if (lix < 45)
        {
            return 3;
        }

        if (lix < 55)
        {
            return 4;
        }

        return 5;
    }

    private string GetContentSource(string url)
    {
        string pattern = @"^(?:.*:\/\/)?(?:www\.)?(?<site>[^:\/]*).*$";
        Regex rgx = new Regex(pattern);
        Match m = rgx.Match(url);
        var contentSource = "";
        if (m.Success)
        {
            return contentSource = m.Groups[1].Value;
        }

        return "FAILED";
    }
}