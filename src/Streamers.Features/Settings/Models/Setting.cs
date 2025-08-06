using Shared.Abstractions.Domain;

namespace Streamers.Features.Settings.Models;

public class Setting : Entity
{
    public bool EmailNotificationsEnabled { get; set; }
}
