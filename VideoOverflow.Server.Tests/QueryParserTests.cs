using FluentAssertions;
using VideoOverflow.Infrastructure.repositories;
using VideoOverflow.Infrastructure.Tests;

namespace VideoOverflow.Server.Tests;

/// <summary>
/// Tests for our queryParser
/// </summary>
[Collection("Database")]
public class QueryParserTests : DatabaseTestCase {
    private readonly TagRepository _tagRepo;

    public QueryParserTests(DatabaseTemplateFixture databaseFixture) : base(databaseFixture) {
        _tagRepo = new TagRepository(DbContext);
    }
    
    [Fact]
    public async Task QueryParser_given_how_to_start_docker_linux_and_tags_Docker_Linux_Git_returns_Docker_Linux() {
        await _tagRepo.Push(new TagCreateDTO() {Name = "Docker", TagSynonyms = new List<string>()});
        await _tagRepo.Push(new TagCreateDTO() {Name = "Linux", TagSynonyms = new List<string>()});
        await _tagRepo.Push(new TagCreateDTO() {Name = "Git", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo);
        var expected = new List<TagDTO>() {
            new (1, "Docker", new List<string>()),
            new (2, "Linux", new List<string>())
        };
        var actual = parser.ParseTags("how to start docker linux").ToList();
        expected.Should().BeEquivalentTo(actual);
    }


    [Fact]
    public async Task QueryParser_given_Hi_john_and_tag_John_returns_John() {
        await _tagRepo.Push(new TagCreateDTO() {Name = "John", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo);
        var expected = new List<TagDTO>() {new (1, "John", new List<string>())};
        var actual = parser.ParseTags("Hi john").ToList();
        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task QueryParser_given_query_start_docker_container_with_bash_commands_returns_match_percent() {
        await _tagRepo.Push(new TagCreateDTO() {Name = "Git", TagSynonyms = new List<string>(){"Github"}});
        await _tagRepo.Push(new TagCreateDTO() {Name = "Command", TagSynonyms = new List<string>(){"Commands", "CommandLine"}});
        await _tagRepo.Push(new TagCreateDTO() {Name = "Bash", TagSynonyms = new List<string>(){"Zsh", "Sh", "Shell", "Bourne-Again"}});
    
        var parser = new QueryParser(_tagRepo);
        var expected = new List<TagDTO>() {
            new (2, "Command", new List<string>(){"Commands", "CommandLine"}),
            new (3, "Bash", new List<string>(){"Zsh", "Sh", "Shell", "Bourne-Again"})
        };
        var actual = parser.ParseTags("start docker container with bash commands").ToList();
        expected.Should().BeEquivalentTo(actual);
    }
    
    [Fact]
    public async Task QueryParser_given_query_undo_commit_git_command_line_returns_Commit_Command() {
        await _tagRepo.Push(new TagCreateDTO() {Name = "Git", TagSynonyms = new List<string>(){"Github"}});
        await _tagRepo.Push(new TagCreateDTO() {Name = "Command", TagSynonyms = new List<string>(){"Commands", "CommandLine"}});
        await _tagRepo.Push(new TagCreateDTO() {Name = "Bash", TagSynonyms = new List<string>(){"Zsh", "Sh", "Shell", "Bourne-Again"}});
    
        var parser = new QueryParser(_tagRepo);
        var expected = new List<TagDTO>() {
            new (1, "Git", new List<string>(){"Github"}),
            new (2, "Command", new List<string>(){"Commands", "CommandLine"})
        };
        var actual = parser.ParseTags("undo commit git command line").ToList();
        expected.Should().BeEquivalentTo(actual);
    }
}