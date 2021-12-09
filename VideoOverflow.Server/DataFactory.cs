using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using VideoOverflow.Infrastructure.Context;
using VideoOverflow.Infrastructure.Entities;

namespace Server;

public static class DataFactory
{
    public static async Task<IHost> FillDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VideoOverflowContext>();

            await CreateDemoData(context);
        }

        return host;
    }

    private static async Task CreateDemoData(VideoOverflowContext context)
    {
        await context.Database.MigrateAsync();

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
        GenerateComments(context);
        
        // Attach the comments to list of users and resources
        await AttachCommentsToUsersAndResources(context);
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

    private static async Task AttachCommentsToUsersAndResources(VideoOverflowContext context)
    {
        foreach (var user in context.Users)
        {
            foreach (var comment in context.Comments)
            {
                if (comment.CreatedBy == user.Id)
                {
                    user.Comments.Add(comment);
                }
            }
        }
        
        foreach (var resource in context.Resources)
        {
            foreach (var comment in context.Comments)
            {
                if (comment.CreatedBy == resource.Id)
                {
                    resource.Comments.Add(comment);
                }
            }
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
            new DateTime(2020, 9, 29),
            new DateTime(2019, 9, 23),
            new DateTime(2020, 8, 14),
            new DateTime(2021, 11, 15),
            new DateTime(2021, 7, 0),
            new DateTime(2021, 11, 10),
            new DateTime(2018, 11, 5),
            new DateTime(2020, 12, 23),
            new DateTime(2019, 3, 20),
            new DateTime(2020, 10, 28),
            new DateTime(2021, 10, 10)
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
