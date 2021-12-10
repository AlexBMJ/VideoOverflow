using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using VideoOverflow.Core.DTOs;
using Xunit;

namespace VideoOverflow.Server.Integration.Tests;

/* Test setup has been taken from: https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Server.Integration.Tests/CharacterTests.cs */
public class ResourceTests : IClassFixture<VideoOverflowWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly VideoOverflowWebApplicationFactory _factory;

    public ResourceTests(VideoOverflowWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_returns_resources()
    {
        var resources = await _client.GetFromJsonAsync<ResourceDTO[]>("api/resources");

        resources.Should().NotBeNull();
        resources.Should().HaveCount(12);
        resources.Should()
            .Contain(
                resource => resource.SiteUrl == "https://www.youtube.com/watch?v=PQsJR8ci3J0&ab_channel=edureka%21");
        resources.Should().Contain(res => res.Author == "Programming with Mosh");
        
    }

    [Theory]
    [InlineData("edureka!")]
    [InlineData("dotNET")]
    [InlineData("Docker")]
    [InlineData("Java")]
    [InlineData("TechTarget")]
    [InlineData("wikipedia")]
    [InlineData("Andy Sterkowitz")]
    [InlineData("geeksforgeeks")]
    [InlineData("Programming with Mosh")]
    [InlineData("Martin Kleppman")]
    [InlineData("Unknown")]
    public async Task Get_returns_all_authors(string author)
    {
        var resources = await _client.GetFromJsonAsync<ResourceDTO[]>("api/Resources");

        foreach (var resourceDto in resources)
        {
            Assert.Equal(author, resourceDto.Author);
        }
    }
}