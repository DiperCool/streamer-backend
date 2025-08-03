using Streamers.Api.Profiles.Models;
using Streamers.Api.Settings.Models;

namespace Streamers.Api.Streamers.Models;

public class Streamer(Guid id, string userName, string email, string firstName, string lastName,  Profile profile, Setting setting)
{
    public Guid Id { get; set; } = id;
    public string UserName { get; set; } = userName;
    public string Email { get; set; } = email;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public long Followers { get; set; }
    
    public Setting Setting { get; set; } = setting;
    public Profile Profile { get; set; } = profile;
}
