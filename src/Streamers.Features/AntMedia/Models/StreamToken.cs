namespace Streamers.Features.AntMedia.Models;

public class StreamToken
{
    public string Ip { get; set; } = null!;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Action { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string Query { get; set; } = string.Empty;
}
