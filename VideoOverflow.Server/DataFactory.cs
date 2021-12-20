
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using FluentAssertions.Extensions;
using Microsoft.VisualBasic;
using VideoOverflow.Infrastructure.Context;
using VideoOverflow.Infrastructure.Entities;

namespace Server;

/// <summary>
/// Factory to seed the database
/// </summary>
public static class DataFactory
{
    private static IList<string> _categories = new List<string>();
    private static IList<string> _tags = new List<string>();
    private static IList<ResourceType> _resourceTypes = new List<ResourceType>() {ResourceType.Article, ResourceType.Book, ResourceType.Video };
    private static IList<string> _languages = new List<string>() {"Danish", "English"};

    private static IList<string> _authors = new List<string>()
    {
        "edureka!", "dotNET", "Docker", "Java", "TechTarget", "wikipedia", "Andy Sterkowitz",
        "geeksforgeeks", "Programming with Mosh", "Martin Kleppman", "Unknown"
    };

    private static IList<string> _urls = new List<string>()
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

    private static IList<string> _siteTitles = new List<string>()
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
    private static readonly int _amountOfResources = 1000;
    private static Random rnd = new Random();
    private static readonly int _dateRange = 30*365; //30 years
    /// <summary>
    /// Fills the database based on the context made from the user secrets
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static async Task<IHost> FillDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VideoOverflowContext>();
            await context.Database.EnsureDeletedAsync();
            await CreateDemoData(context);
        }

        return host;
    }

    /// <summary>
    /// Creates data for the database and fills it
    /// </summary>
    /// <param name="context">The context for the database</param>
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

        var categories = new List<Category>()
        {
            new Category() {Name = "Programming"},
            new Category() {Name = "Testing"},
            new Category() {Name = "Database"},
            new Category() {Name = "Distributed Systems"},
            new Category() {Name = "UX"},
            new Category() {Name = "Algorithms"},
            new Category() {Name = "Data structures"},
            new Category() {Name = "Version control"},
            new Category() {Name = "Containerization"},
            new Category() {Name = "UI"}
        };
        // Add categories to the database
        await context.Categories.AddRangeAsync(
           categories
        );
       _categories = categories.Select(x => x.Name).ToList();

       var tags = new List<Tag>()
       {
           new Tag() {Name = "C#"}, new Tag() {Name = "Java"}, new Tag() {Name = "Python"},
           new Tag() {Name = "Microsoft"}, new Tag() {Name = "JavaScript"}, new Tag() {Name = "C++"},
           new Tag() {Name = "C"}, new Tag() {Name = "Docker"}, new Tag() {Name = "LearnIT"},
           new Tag() {Name = "Git"}, new Tag() {Name = "SQL"}
       };
        // Add tags to the database
        await context.Tags.AddRangeAsync(
            tags
        );
        _tags = tags.Select(x => x.Name).ToList();

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

    /// <summary>
    /// Generates a bunch of comments based on some predefined ones
    /// </summary>
    /// <param name="context">The context of the DB</param>
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
        

        for (int i = 0; i < 2000; i++)
        {
            await context.Comments.AddAsync(new Comment()
            {
                Content = commentContent[rnd.Next(0, commentContent.Count - 1)],
                CreatedBy = rnd.Next(1, context.Users.Count()),
                AttachedToResource = rnd.Next(1, context.Resources.Count())
            });
        }
    }

    /// <summary>
    /// Attaches the generated comments to users and resources
    /// </summary>
    /// <param name="context">The context for the DB</param>
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
    /// <summary>
    /// Generates a predefined amount of resources based on some predefined tags, categories, siteurls etc.
    /// </summary>
    /// <param name="context">The context for the DB</param>
    private static async Task GenerateResources(VideoOverflowContext context)
    {
        var siteTitles = new Collection<string>();

        var siteUrls = new Collection<string>();

        var authors = new Collection<string>();

        var createdDates = new Collection<DateTime>();

        var categories = new Collection<ICollection<string>>();

        PopulateList(ref _categories, ref categories);
        
        var lixNumbers = new Collection<int>();

        var languages = new Collection<string>();

        var tags = new Collection<ICollection<string>>();
        
        PopulateList(ref _tags, ref tags);

        var materialTypes = new Collection<ResourceType>();
        
        for (int i = 0; i < _amountOfResources; i++)
        {
            var randomDate = DateTime.Today.AddDays(- rnd.Next(_dateRange));
            var materialIndex = rnd.Next(0, _resourceTypes.Count);
            var languageIndex = rnd.Next(0, _languages.Count);
            var authorIndex = rnd.Next(0, _authors.Count);
            var urlIndex = rnd.Next(0, _urls.Count);
            var titleIndex = rnd.Next(0, _siteTitles.Count);
            materialTypes.Add(_resourceTypes[materialIndex]);
            languages.Add(_languages[languageIndex]);
            createdDates.Add(randomDate.AsUtc());
            authors.Add(_authors[authorIndex]);
            siteUrls.Add(_urls[urlIndex]);
            siteTitles.Add(_siteTitles[titleIndex]);
            lixNumbers.Add(rnd.Next(10,100));
        }

        for (int i = 0; i < _amountOfResources; i++)
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

    /// <summary>
    /// Finds all the tags in the DB that correlates to the names from a collection
    /// </summary>
    /// <param name="context">The context of the DB</param>
    /// <param name="tagNames">The collection of tag names</param>
    /// <returns>All tags in the DB from the tag names</returns>
    private static Collection<Tag> FindTags(VideoOverflowContext context, ICollection<string> tagNames)
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

    /// <summary>
    /// Finds all the categories in the DB that correlates to the names from a collection
    /// </summary>
    /// <param name="context">The context for the DB</param>
    /// <param name="categories">The collection of category names</param>
    /// <returns>All categories in the DB from the category names</returns>
    private static Collection<Category> FindCategories(VideoOverflowContext context, ICollection<string> categories)
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

    /// <summary>
    /// Gets the skillLevel based on the lix
    /// </summary>
    /// <param name="lix">The lixnumber</param>
    /// <returns>The skillLevel based on the lixNumber</returns>
    private static int GetSkillLevel(int lix)
    {
        return lix < 25 ? 1 : lix < 35 ? 2 : lix < 45 ? 3 : lix < 55 ? 4 : 5;
    }

    /// <summary>
    /// Gets the contentSource from a url
    /// </summary>
    /// <param name="url">A url to get the contentSource from</param>
    /// <returns>The contentSource from the url</returns>
    private static string GetContentSource(string url)
    {
        return new Regex(@"^(?:.*:\/\/)?(?:www\.)?(?<site>[^:\/]*).*$").Match(url).Groups[1].Value;
    }

    /// <summary>
    /// Populates a list with a random amount of items from another list
    /// </summary>
    /// <param name="populateWith">The list to get items from</param>
    /// <param name="toPopulate">The list to populate</param>
    /// <typeparam name="T">The type of the two lists</typeparam>
    private static void PopulateList<T>(ref IList<T> populateWith, ref Collection<ICollection<T>> toPopulate)
    {
        for (var i = 0; i < _amountOfResources; i++)
        {
            ICollection<T> toAdd = new List<T>();
            var amount = rnd.Next(0, populateWith.Count);
            var index = rnd.Next(0, populateWith.Count);
            for (var j = 0; j < amount; j++)
            {
                toAdd.Add(populateWith[index]);
                if(amount != 1) {
                    index = (index + rnd.Next(1, (populateWith.Count) / (amount-1))) % populateWith.Count;
                }
            }
            toPopulate.Add(toAdd);
        }
    }
}
