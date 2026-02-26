using Shared.Abstractions.Domain;
using Streamers.Features.Categories.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Tags.Models;

namespace Streamers.Features.StreamInfos.Models;

public record StreamTitleChanged(string StreamerId, string NewTitle, string ModeratorId)
    : IDomainEvent;

public record StreamCategoryChanged(string StreamerId, Guid? NewCategoryId, string ModeratorId)
    : IDomainEvent;

public record StreamLanguageChanged(string StreamerId, string NewLanguage, string ModeratorId)
    : IDomainEvent;

public class StreamInfo : Entity
{
    protected StreamInfo() { }

    public StreamInfo(string? title, string language)
    {
        Title = title;
        Language = language;
    }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? Title { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public string Language { get; set; }

    public void Update(
        string title,
        Category? category,
        List<Tag> tags,
        string language,
        string moderatorId
    )
    {
        if (Language != language)
        {
            Language = language;
            Raise(new StreamLanguageChanged(StreamerId, Language, moderatorId));
        }

        if (Title != title)
        {
            Title = title;
            Raise(new StreamTitleChanged(StreamerId, Title, moderatorId));
        }

        if (CategoryId != category?.Id)
        {
            Category = category;
            CategoryId = category?.Id;
            Raise(new StreamCategoryChanged(StreamerId, CategoryId, moderatorId));
        }

        Tags.Clear();
        Tags.AddRange(tags);
    }
}
