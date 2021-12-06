using System.Collections.Generic;
using System.Linq;
using Server;

namespace VideoOverflow.Server.Tests;

public class QueryParserTests
{
    private readonly VideoOverflowContext _context;
    private readonly TagRepository _tagRepo;
    private readonly WordRepository _wordRepo;

    public QueryParserTests()
    {
        var repo = new QueryParserTestsSetup();
        _context = repo.Context;

        _tagRepo = new TagRepository(_context);
        _wordRepo = new WordRepository(_context);
    }
    
    [Fact]
    public void QueryParser_given_Hi_John_and_tags_Hi_and_John_returns_hi_and_john()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        _tagRepo.Push(new TagCreateDTO() {Name = "John", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = new List<string>() {"hi", "john"};
        var actual = parser.Parse("Hi John").ToList();
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
    }
    
    [Fact]
    public void QueryParser_given_Hi_John_and_tags_Hi_returns_hi()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = new List<string>() {"hi"};
        var actual = parser.Parse("Hi John").ToList();
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equal(expected[0], actual[0]);
    }
    
    [Fact]
    public void QueryParser_given_He_Jon_and_tags_Hi_and_John_and_no_words_returns_Hi_John()
    {
        _tagRepo.Push(new TagCreateDTO() {Name = "Hi", TagSynonyms = new List<string>()});
        _tagRepo.Push(new TagCreateDTO() {Name = "John", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = "Hi John";
        var actual = parser.SuggestQuery("He Jon");
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void QueryParser_given_Hi_Jonny_and_tag_Johnny_and_word_Hi_returns_Hi_Johnny()
    {
        _wordRepo.Push(new WordCreateDTO(){Hash = "Hi".GetHashCode(), String = "Hi"});
        _tagRepo.Push(new TagCreateDTO() {Name = "Johnny", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = "Hi Johnny";
        var actual = parser.SuggestQuery("Hi Jonny");
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void QueryParser_given_Dokor_cs_sofware_engineering_and_tag_Docker_CSharp_and_words_Software_Engineering_returns_Docker_csharp_Software_Engineering()
    {
        _wordRepo.Push(new WordCreateDTO(){Hash = "Hi".GetHashCode(), String = "Hi"});
        _tagRepo.Push(new TagCreateDTO() {Name = "Johnny", TagSynonyms = new List<string>()});
        var parser = new QueryParser(_tagRepo, _wordRepo);
        var expected = "Hi Johnny";
        var actual = parser.SuggestQuery("Hi Jonny");
        Assert.Equal(expected, actual);
    }
}