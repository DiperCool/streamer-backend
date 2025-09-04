namespace Shared.Auth0.Services;

public interface IAuth0Service
{
    Task<bool> UserExists(string email);
    Task<string> CreateUser(string id, string email, string userName, string password);
    Task<string> GetAccessToken();
    Task AssignRole(string id, string[] roles);
}
