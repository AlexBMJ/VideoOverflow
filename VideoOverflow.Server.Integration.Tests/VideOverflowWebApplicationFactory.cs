using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using FluentAssertions.Extensions;
using Server;
using VideoOverflow.Core;
using VideoOverflow.Infrastructure.Context;
using VideoOverflow.Infrastructure.Entities;

namespace VideoOverflow.Server.Integration.Tests;

public class VideoOverflowWebApplicationFactory : WebApplicationFactory<Program>
{
    /* This method has been taken from: https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Server.Integration.Tests/CustomWebApplicationFactory.cs */
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VideoOverflowContext>));

            if (dbContext != null)
            {
                services.Remove(dbContext);
            }

            /* Overriding policies and adding Test Scheme defined in TestAuthHandler */
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Test")
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            
            var connection = new SqliteConnection("Filename=:memory:");

            services.AddDbContext<VideoOverflowContext>(options =>
            {
                options.UseSqlite(connection);
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<VideoOverflowContext>();
            appContext.Database.OpenConnection();
            appContext.Database.EnsureCreated();
            FillDatabase(appContext);
        });
        
        builder.UseEnvironment("Integration");

        return base.CreateHost(builder);
    }
    private static async Task FillDatabase(VideoOverflowContext context)
    {
       
        // Add users to the database
        await context.Users.AddRangeAsync(
            new User() {Name = "Deniz", Comments = new Collection<Comment>()},
            new User() {Name = "Anton", Comments = new Collection<Comment>()},
            new User() {Name = "Christian", Comments = new Collection<Comment>()},
            new User() {Name = "Karl", Comments = new Collection<Comment>()},
            new User() {Name = "Asmus", Comments = new Collection<Comment>()},
            new User() {Name = "Alex", Comments = new Collection<Comment>()}
        );

        // Add categories to the database
        await context.Categories.AddRangeAsync(
            new Category() {Name = "Programming"},
            new Category() {Name = "Object Oriented Programming"},
            new Category() {Name = "Functional Programming"},
            new Category() {Name = "Testing"},
            new Category() {Name = "Unit Testing"},
            new Category() {Name = "Database"},
            new Category() {Name = "Distributed Systems"},
            new Category() {Name = "UX"},
            new Category() {Name = "Algorithm and DataStructures"},
            new Category() {Name = "Github"},
            new Category() {Name = "Docker"},
            new Category() {Name = "Kattis"}
        );

        // Add tags to the database
        await context.Tags.AddRangeAsync(
            new Tag() {Name = "C#"}, new Tag() {Name = "Java"}, new Tag() {Name = "Python"},
            new Tag() {Name = "Microsoft"}, new Tag() {Name = "JavaScript"}, new Tag() {Name = "C++"},
            new Tag() {Name = "C"}, new Tag() {Name = "Docker"}, new Tag() {Name = "LearnIT"}, 
            new Tag() {Name = "Git"}, new Tag() { Name = "SQL"}
        );

        // Save changes before creating tagsynonyms
        await context.SaveChangesAsync();

        // Add tagsyonyms
        await context.TagSynonyms.AddRangeAsync(
            new TagSynonym() {Name = "CS", Tags = FindTags(context, new Collection<string>() {"C#"})},
            new TagSynonym() {Name = "C-sharp", Tags = FindTags(context, new Collection<string>() {"C#"})},
            new TagSynonym() {Name = "PY", Tags = FindTags(context, new Collection<string>() {"Python"})},
            new TagSynonym() {Name = "JS", Tags = FindTags(context, new Collection<string>() {"JavaScript"})}
        );

        // Add resources
        await GenerateResources(context);
        
        // Get the created comments
        await GenerateComments(context);
        
        // Attach the comments to list of users and resources
        await AttachCommentsToUsersAndResources(context);

        await context.SaveChangesAsync();

    }

    private static async Task GenerateComments(VideoOverflowContext context)
    {
        var commentContent = new Collection<string>()
        {
            "Thank you for the udemy courses! Sir, you are great :)",
            "Nice Tutorial!",
            "Who stole my beer!",
            "I can use this, Thank you!",
            "This tutorial will fix my problem, thx!",
            "Can this help me find deleted text messages?"
        };

        var randomNumberGenerator = new Random();

        for (int i = 0; i < 2000; i++)
        {
            await context.Comments.AddAsync(new Comment()
            {
                Content = commentContent[randomNumberGenerator.Next(0, commentContent.Count - 1)],
                CreatedBy = randomNumberGenerator.Next(1, context.Users.Count()),
                AttachedToResource = randomNumberGenerator.Next(1, context.Resources.Count())
            });
        }
    }

    private static async Task AttachCommentsToUsersAndResources(VideoOverflowContext context) {
        var comments = from user in context.Users
            join comment in context.Comments
                on user.Id equals comment.Id
            select new { Owner = user, Comment = comment };

        foreach (var comment in comments) {
            comment.Owner.Comments?.Add(comment.Comment);
        }
        
        var resources = from resource in context.Resources
            join comment in context.Comments
                on resource.Id equals comment.CreatedBy
            select new { CreatedBy = resource, Comment = comment };

        foreach (var resource in resources) {
            resource.CreatedBy.Comments?.Add(resource.Comment);
        }

        await context.SaveChangesAsync();
    }

    // The indexes of each list to generate a resource that matches the real information.
    private static async Task GenerateResources(VideoOverflowContext context)
    {
        var siteTitles = new Collection<string>()
        {
            "How to use GitHub | What is GitHub | Git and GitHub Tutorial | DevOps Training | Edureka",
            "What is C#? | C# 101 [1 of 19]",
            "How to Get Started with Docker",
            "The Java Tutorial",
            "object-oriented programming (OOP)",
            "Functional programming",
            "What is Unit Testing? Why YOU Should Learn It + Easy to Understand Examples",
            "Types of Software Testing",
            "MySQL Tutorial for Beginners [Full Course]",
            "Distributed Systems 1.1: Introduction",
            "User Experience (UX) Design"
        };

        var siteUrls = new Collection<string>()
        {
            "https://www.youtube.com/watch?v=PQsJR8ci3J0&ab_channel=edureka%21",
            "https://www.youtube.com/watch?v=BM4CHBmAPh4&list=PLdo4fOcmZ0oVxKLQCHpiUWun7vlJJvUiN&ab_channel=dotNET",
            "https://www.youtube.com/watch?v=iqqDU2crIEQ&t=38s&ab_channel=Docker",
            "https://docs.oracle.com/javase/tutorial/",
            "https://searchapparchitecture.techtarget.com/definition/object-oriented-programming-OOP",
            "https://en.wikipedia.org/wiki/Functional_programming",
            "https://www.youtube.com/watch?v=3kzHmaeozDI&ab_channel=AndySterkowitz",
            "https://www.geeksforgeeks.org/types-software-testing/",
            "https://www.youtube.com/watch?v=7S_tz1z_5bA&t=279s",
            "https://www.youtube.com/watch?v=UEAMfLPZZhE&ab_channel=MartinKleppmann",
            "https://www.interaction-design.org/literature/topics/ux-design"
        };

        var authors = new Collection<string>()
        {
            "edureka!", "dotNET", "Docker", "Java", "TechTarget", "wikipedia", "Andy Sterkowitz",
            "geeksforgeeks", "Programming with Mosh", "Martin Kleppman", "Unknown"
        };

        var createdDates = new Collection<DateTime>()
        {
            DateTime.Parse("2020-09-29").AsUtc(),
            DateTime.Parse("2019-09-23").AsUtc(),
            DateTime.Parse("2020-08-14").AsUtc(),
            DateTime.Parse("2021-11-15").AsUtc(),
            DateTime.Parse("2021-07-01").AsUtc(),
            DateTime.Parse("2021-11-10").AsUtc(),
            DateTime.Parse("2018-11-05").AsUtc(),
            DateTime.Parse("2020-12-23").AsUtc(),
            DateTime.Parse("2019-03-20").AsUtc(),
            DateTime.Parse("2020-10-28").AsUtc(),
            DateTime.Parse("2021-10-10").AsUtc(),
            
        };

        var categories = new Collection<Collection<string>>()
        {
            new Collection<string>() {"Github"}, new Collection<string>() {"Object Oriented Programming"},
            new Collection<string>() {"Docker"}, new Collection<string>() {"Object Oriented Programming"},
            new Collection<string>() {"Object Oriented Programming"},
            new Collection<string>() {"Functional Programming"},
            new Collection<string>() {"Testing", "Unit Testing"},
            new Collection<string>()
                {"Testing", "Unit Testing", "Object Oriented Programming", "Functional Programming"},
            new Collection<string>() {"Database"}, new Collection<string>() {"Distributed Systems"},
            new Collection<string>() {"UX"}
        };

        var lixNumbers = new Collection<int>() {10, 25, 40, 50, 30, 40, 40, 55, 25, 25, 30};

        var languages = new Collection<string>()
        {
            "English", "English", "English", "English", "English", "English", "English", "English", "English",
            "English", "English"
        };

        var tags = new Collection<Collection<string>>()
        {
            new Collection<string>() {"Git"}, new Collection<string>() {"C#"},
            new Collection<string>() { }, new Collection<string>() {"Java"},
            new Collection<string>() {"Java", "C#", "C++"}, new Collection<string>() { },
            new Collection<string>() { }, new Collection<string>() { }, new Collection<string>() {"SQL"},
            new Collection<string>() { }, new Collection<string>() { }
        };

        var materialTypes = new Collection<ResourceType>()
        {
            ResourceType.VIDEO, ResourceType.VIDEO, ResourceType.VIDEO, ResourceType.ARTICLE,
            ResourceType.ARTICLE, ResourceType.ARTICLE, ResourceType.VIDEO, ResourceType.ARTICLE,
            ResourceType.VIDEO, ResourceType.VIDEO, ResourceType.ARTICLE
        };

        for (int i = 0; i < siteTitles.Count; i++)
        {
            await context.Resources.AddAsync(new Resource
            {
                SiteTitle = siteTitles[i],
                SiteUrl = siteUrls[i],
                Author = authors[i],
                Categories = FindCategories(context, categories[i]),
                Created = createdDates[i],
                MaterialType = materialTypes[i],
                Comments = new Collection<Comment>(),
                ContentSource = GetContentSource(siteUrls[i]),
                Language = languages[i],
                Tags = FindTags(context, tags[i]),
                LixNumber = lixNumbers[i],
                SkillLevel = GetSkillLevel(lixNumbers[i]),
            });
        }

        await context.SaveChangesAsync();
    }

    private static Collection<Tag> FindTags(VideoOverflowContext context, Collection<string> tagNames)
    {
        var tags = new Collection<Tag>();

        foreach (var tagName in tagNames)
        {
            foreach (var tag in context.Tags)
            {
                if (tag.Name == tagName)
                {
                    tags.Add(tag);
                }
            }
        }

        return tags;
    }

    private static Collection<Category> FindCategories(VideoOverflowContext context, Collection<string> categories)
    {
        var categoriesFound = new Collection<Category>();

        foreach (var categoryName in categories)
        {
            foreach (var category in context.Categories)
            {
                if (category.Name == categoryName)
                {
                    categoriesFound.Add(category);
                }
            }
        }

        return categoriesFound;
    }

    private static int GetSkillLevel(int lix)
    {
        return lix < 25 ? 1 : lix < 35 ? 2 : lix < 45 ? 3 : lix < 55 ? 4 : 5;
    }

    private static string GetContentSource(string url)
    {
        return new Regex(@"^(?:.*:\/\/)?(?:www\.)?(?<site>[^:\/]*).*$").Match(url).Groups[1].Value;
    }
}
