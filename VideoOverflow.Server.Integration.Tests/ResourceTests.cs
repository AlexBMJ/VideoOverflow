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
        var resources = await _client.GetFromJsonAsync<ResourceDTO[]>("api/Resource");

        resources.Should().NotBeNull();
        resources.Should().HaveCount(11);
        resources.Should()
            .Contain(
                resource => resource.SiteUrl == "https://www.youtube.com/watch?v=PQsJR8ci3J0&ab_channel=edureka%21");
        resources.Should().Contain(res => res.Author == "Programming with Mosh");
        
    }

    [Fact]
    public async Task Get_returns_all_authors()
    {
        var resources = await _client.GetFromJsonAsync<ResourceDTO[]>("api/Resource");


        var expectedAuthors = new[]
        {
            "edureka!", "dotNET", "Docker", "Java", "TechTarget", "wikipedia",
            "Andy Sterkowitz", "geeksforgeeks", "Programming with Mosh", "Martin Kleppman", "Unknown"
        };

        for (int i = 0; i < resources.Length; i++)
        {
                Assert.Equal(expectedAuthors[i], resources[i].Author);
        }
    }
}