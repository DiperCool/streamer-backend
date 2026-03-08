using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Streamers.Features.Tags.Models;
using Streamers.Features.Tags.Services;
using Xunit;

namespace Streamers.Features.IntegrationTests.Tags;

public class TagsServiceTests : BaseIntegrationTest
{
    private readonly ITagsService _tagsService;

    public TagsServiceTests(StreamerWebApplicationFactory factory)
        : base(factory)
    {
        _tagsService = _scope.ServiceProvider.GetRequiredService<ITagsService>();
    }

    [Fact]
    public async Task Create_ShouldCreateNewTags()
    {
        // Arrange
        var tagTitles = new List<string> { "Gaming", "Music", "Art" };

        // Act
        var createdTags = await _tagsService.Create(tagTitles);

        // Assert
        createdTags.Should().NotBeNullOrEmpty().And.HaveCount(3);
        createdTags.Should().Contain(t => t.Title == "Gaming");
        createdTags.Should().Contain(t => t.Title == "Music");
        createdTags.Should().Contain(t => t.Title == "Art");

        var tagsInDb = await DbContext.Tags.Select(t => t.Title).ToListAsync();
        tagsInDb.Should().HaveCount(3);
        tagsInDb.Should().Contain("Gaming", "Music", "Art");
    }

    [Fact]
    public async Task Create_ShouldReturnExistingTagsAndCreateNewOnes()
    {
        // Arrange
        await DbContext.Tags.AddAsync(new Tag(Guid.NewGuid(), "Gaming"));
        await DbContext.SaveChangesAsync();

        var tagTitles = new List<string> { "Gaming", "Music", "Art" };

        // Act
        var createdTags = await _tagsService.Create(tagTitles);

        // Assert
        createdTags.Should().NotBeNullOrEmpty().And.HaveCount(3);
        createdTags.Should().Contain(t => t.Title == "Gaming");
        createdTags.Should().Contain(t => t.Title == "Music");
        createdTags.Should().Contain(t => t.Title == "Art");

        var tagsInDb = await DbContext.Tags.ToListAsync();
        tagsInDb.Should().HaveCount(3);
        tagsInDb.Should().Contain(t => t.Title == "Gaming");
        tagsInDb.Should().Contain(t => t.Title == "Music");
        tagsInDb.Should().Contain(t => t.Title == "Art");
    }

    [Fact]
    public async Task Create_ShouldHandleDuplicateTitlesCaseInsensitive()
    {
        // Arrange
        var tagTitles = new List<string> { "gaming", "Gaming", "GAMING", "Music" };

        // Act
        var createdTags = await _tagsService.Create(tagTitles);

        // Assert
        createdTags.Should().NotBeNullOrEmpty().And.HaveCount(2); // "gaming" and "Music"
        createdTags.Should().Contain(t => t.Title.Equals("gaming", StringComparison.OrdinalIgnoreCase));
        createdTags.Should().Contain(t => t.Title == "Music");

        var tagsInDb = await DbContext.Tags.ToListAsync();
        tagsInDb.Should().HaveCount(2);
        tagsInDb.Should().Contain(t => t.Title.Equals("gaming", StringComparison.OrdinalIgnoreCase));
        tagsInDb.Should().Contain(t => t.Title == "Music");
    }

    [Fact]
    public async Task Create_ShouldIgnoreWhitespaceAndEmptyTitles()
    {
        // Arrange
        var tagTitles = new List<string> { "  Gaming  ", "", " ", "Music" };

        // Act
        var createdTags = await _tagsService.Create(tagTitles);

        // Assert
        createdTags.Should().NotBeNullOrEmpty().And.HaveCount(2);
        createdTags.Should().Contain(t => t.Title == "Gaming");
        createdTags.Should().Contain(t => t.Title == "Music");

        var tagsInDb = await DbContext.Tags.ToListAsync();
        tagsInDb.Should().HaveCount(2);
        tagsInDb.Should().Contain(t => t.Title == "Gaming");
        tagsInDb.Should().Contain(t => t.Title == "Music");
    }
}
