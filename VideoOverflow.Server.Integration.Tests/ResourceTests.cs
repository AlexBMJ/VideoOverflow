using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using VideoOverflow.Core.DTOs;
using Xunit;

namespace VideoOverflow.Server.Integration.Tests;

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
        var resources = await _client.GetFromJsonAsync<ResourceDTO[]>("/api/resources");

        resources.Should().NotBeNull();
        resources.Should().HaveCount(12);
        resources.Should()
            .Contain(
                resource => resource.SiteUrl == "https://www.youtube.com/watch?v=PQsJR8ci3J0&ab_channel=edureka%21");
        resources.Should().Contain(res => res.Author == "Programming with Mosh");
        
    }

    [Fact]
    public async Task continue_test()
    {
      
    }
}