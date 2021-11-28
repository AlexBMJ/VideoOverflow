using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using SQLitePCL;
using Xunit;

namespace VideoOverflow.Infrastructure.Tests;

public class ResourceRepositoryTests : IDisposable
{
    private readonly VideoOverflowContext _context;
    private readonly ResourceRepository _repo;


    public ResourceRepositoryTests()
    {
        var setup = new RepositoryTestsSetup();
        _context = setup.Context;
        _repo = new ResourceRepository(_context);
        
        /*var csharp = new Tag { Name = "CSharp" };
        var docker = new Tag { Name = "Docker" };
        var java = new Tag { Name = "Java" };

        var cdashsharpsynonym = new TagSynonym { Name = "C-Sharp", Tags = new List<Tag>() { csharp }.AsReadOnly() };
        csharp.TagSynonyms = new List<TagSynonym>() { cdashsharpsynonym };

        var user1comment1 = new Comment { Content = "A random comment" };
        var user1comment2 = new Comment { Content = "Another random comment" };
        var user2comment1 = new Comment { Content = "A new comment from another user" };
        var user3comment1 = new Comment { Content = "I like this video" };

        var user1comments = new List<Comment>() { user1comment1, user1comment2};
        var user2comments = new List<Comment>() { user2comment1 };
        var user3comments = new List<Comment>() { user3comment1 };

        var user1 = new User { Name = "Anton", Comments = user1comments.AsReadOnly() };
        var user2 = new User { Name = "Deniz", Comments = user2comments.AsReadOnly() };
        var user3 = new User { Name = "Asmus", Comments = user3comments.AsReadOnly() };

        var containerization = new Category { Name = "Containerization" };
        var programming = new Category { Name = "Programming" };
        var se = new Category { Name = "Software Engineering" };
        var database = new Category { Name = "Database" };

        var resource1 = new Resource { Categories = new List<Category>() { programming }, Comments = new List<Comment>() { user1comment1 }, Author = "Deniz", Langauge = "English", SkillLevel = 3, LixNumber = 39, Tags = new List<Tag>() { csharp }, Site_title = "C-Sharp tutorial", Site_url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ", MaterialType = "Video", Created = new DateTime(2020, 3, 16, 0, 0, 0), Content_source = "Youtube"};
        var resource2 = new Resource { Categories = new List<Category>() { database, containerization }, Comments = new List<Comment>() { user1comment2, user2comment1 }, Author = "Anton", Langauge = "English", Site_title = "Docker step by step tutorial", Tags = new List<Tag>() { docker }, SkillLevel = 4, LixNumber = 46, Site_url = "https://docs.docker.com/get-started/", MaterialType = "Article", Content_source = "Docker", Created = new DateTime(2019, 3, 16, 0, 0, 0)};
        var resource3 = new Resource { Categories = new List<Category>() { se }, Author = "Alex", Comments = new List<Comment>() { user3comment1 }, Langauge = "English", SkillLevel = 2, LixNumber = 31, Site_title = "Software Engineering Tutorial", Site_url = "https://www.javatpoint.com/software-engineering-tutorial", MaterialType = "Article", Tags = new List<Tag>() { java }, Content_source = "Javatpoint", Created = new DateTime (2018, 4, 25, 0, 0, 0) };

        context.Tags.AddRangeAsync(csharp, docker, java);
        context.Users.AddRangeAsync(user1, user2, user3);
        context.Comments.AddRangeAsync(user1comment1, user1comment2, user2comment1, user3comment1);
        context.Categories.AddRangeAsync(containerization, programming, se, database);
        context.TagSynonyms.AddAsync(cdashsharpsynonym);
        context.Resources.AddRangeAsync(resource1, resource2, resource3);

        context.SaveChangesAsync();

     */

    }

    [Fact]
    public async Task Push_Creates_New_Resource_And_Returns_ResourceDTO_With_Id()
    {
        var resource = new ResourceCreateDTO()
        {
            Created = DateTime.Now,
            Author = "Deniz",
            SiteTitle = "My first Page",
            SiteUrl = "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            ContentSource = "LearnIT",
            Language = "Danish",
            MaterialType = ResourceType.VIDEO,
            Categories = new Collection<string>() {"Programming"},
            Tags = new Collection<string>() {"C#"},
            Comments = new Collection<string>()
        };

        var actual = await _repo.Push(resource);

        var expected = new ResourceDTO(1, ResourceType.VIDEO,
            "https://learnit.itu.dk/pluginfile.php/306649/mod_resource/content/3/06-normalization.pdf",
            "My first Page",
            "Deniz",
            "Danish",
            new Collection<string>() {"C#"},
            new Collection<string>() {"Programming"},
            new Collection<string>());

        expected.Should().BeEquivalentTo(actual);

    }
    public void Dispose()
    {
        _context.Dispose();
    }
}