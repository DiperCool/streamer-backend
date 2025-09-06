using Microsoft.EntityFrameworkCore;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Tags.Models;

namespace Streamers.Features.Tags.Services;

public interface ITagsService
{
    Task<List<Tag>> Create(List<string> tags);
}

public class TagsService(StreamerDbContext dbContext) : ITagsService
{
    public async Task<List<Tag>> Create(List<string> tags)
    {
        var normalizedTags = tags.Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existingTags = await dbContext
            .Tags.Where(t => normalizedTags.Contains(t.Title))
            .ToListAsync();

        var existingTitles = existingTags
            .Select(t => t.Title)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newTags = normalizedTags
            .Where(t => !existingTitles.Contains(t))
            .Select(t => new Tag(Guid.NewGuid(), t))
            .ToList();

        if (newTags.Count > 0)
        {
            await dbContext.Tags.AddRangeAsync(newTags);
            await dbContext.SaveChangesAsync();
        }

        return existingTags.Concat(newTags).ToList();
    }
}
