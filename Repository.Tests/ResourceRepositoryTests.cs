using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository.Infrastructure.Context;
using Repository.Infrastructure.Entities;
using Repository.ResourceRepo;
using Xunit;

namespace Repository.Tests;

public class ResourceRepositoryTests : IDisposable
{
    private readonly VideoOverflowContext _context;
    private readonly ResourceRepository _repo;


    public ResourceRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory");
        connection.Open();
        var builder = new DbContextOptionsBuilder<VideoOverflowContext>();
        builder.UseSqlite(connection);
        var context = new VideoOverflowContext(builder.Options);
        context.Database.EnsureCreated();

        var csharp = new Tag { Name = "CSharp" };
        var docker = new Tag { Name = "Docker" };
        var java = new Tag { Name = "Java" };

        var cdashsharpsynonym = new TagSynonym { Name = "C-Sharp", Tags = new List<Tag>() { csharp }.AsReadOnly() };

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
        var resource2 = new Resource { Categories = new List<Category>() { database, containerization}, Comments = new List<Comment>() { user1comment2, user2comment1}, Author = "Anton", Langauge = "English", Site_title = "Docker step by step tutorial", Tags = new List<Tag>() { docker}, SkillLevel = 4, LixNumber = 46, Site_url = "https://docs.docker.com/get-started/", MaterialType = "Article",  }
    }

    [Fact]
    public void Test1()
    {

    }
    public void Dispose()
    {
        _context.Dispose();
    }
}