using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using FluentAssertions;

namespace VideoOverflow.Server.Tests;

public class QueryParserTests
{
    private readonly VideoOverflowContext _context;
    private readonly TagRepository _tagRepo;
    private readonly ResourceRepository _resourceRepo;

    public QueryParserTests() {
        var repo = new QueryParserTestsSetup();
        _context = repo.Context;

        _tagRepo = new TagRepository(_context);
        _resourceRepo = new ResourceRepository(_context);
    }
    
    [Fact]
    public void QueryParser_given_how_to_start_docker_linux_and_tags_Docker_Linux_returns_Docker_Linux()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Docker", TagSynonyms = new List<string>()});
        _tagRepo.Push(new TagCreateDTO() {Name = "Linux", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _resourceRepo);
        var expected = new List<string>() {"docker", "linux"};
        var actual = parser.ParseTags("how to start docker linux").ToList();
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expected[0], actual[0].Name);
        Assert.Equal(expected[1], actual[1].Name);
    }
    
    [Fact]
    public void QueryParser_given_Hi_John_and_tags_Hi_returns_hi()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _resourceRepo);
        var expected = new List<TagDTO>() {new TagDTO(1, "Hi", new List<string>())};
        var actual = parser.ParseTags("Hi John").ToList();
        //Assert.Equal(expected.Count, actual.Count);
        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public void QueryParser_given_query_undo_commit_git_command_line_returns_match_percent()
    {
        _resourceRepo.Push(new ResourceCreateDTO(){SiteTitle = "How to revert git commits", Tags = new List<string>(){"git", "bash", "linux", "windows", "command-line"}});
        _resourceRepo.Push(new ResourceCreateDTO(){SiteTitle = "How to revert git commits", Tags = new List<string>(){"git", "bash", "linux", "windows", "command-line"}});
        _resourceRepo.Push(new ResourceCreateDTO(){SiteTitle = "How to revert git commits", Tags = new List<string>(){"git", "bash", "linux", "windows", "command-line"}});

        _tagRepo.Push(new TagCreateDTO() {Name = "git", TagSynonyms = new List<string>(){"github"}});
        _tagRepo.Push(new TagCreateDTO() {Name = "command", TagSynonyms = new List<string>(){"commands", "command-line"}});
        _tagRepo.Push(new TagCreateDTO() {Name = "bash", TagSynonyms = new List<string>(){"zsh", "sh", "shell", "bourne-again"}});

        var parser = new QueryParser(_tagRepo, _resourceRepo);
        var expected = "git";
        var actual = parser.ParseTags("how to start docker linux").ToList();
        Assert.Equal(expected, actual[0].Name);
    }
}