using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using streamer.ServiceDefaults;

namespace Shared.Auth0.Services;

public class Auth0Service(IConfiguration configuration, IConnectionMultiplexer redis)
    : IAuth0Service
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string AccessTokenKey = "auth0:management:access_token";

    public async Task<bool> UserExists(string id)
    {
        try
        {
            var client = await GetClient();
            await client.Users.GetAsync(id);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string> CreateUser(string id, string email, string userName, string password)
    {
        var client = await GetClient();
        var user = await client.Users.CreateAsync(
            new UserCreateRequest()
            {
                UserId = id,
                Email = email,
                NickName = userName,
                FirstName = userName,
                LastName = userName,
                Password = password,
                Connection = "Username-Password-Authentication",
            }
        );

        return user.UserId;
    }

    public async Task<string> GetAccessToken()
    {
        var cachedToken = await _db.StringGetAsync(AccessTokenKey);
        if (!cachedToken.IsNullOrEmpty)
        {
            return cachedToken!;
        }

        var options = configuration.BindOptions<Auth0Options>();
        var client = new AuthenticationApiClient(new Uri(options.AuthUrl));

        var response = await client.GetTokenAsync(
            new ClientCredentialsTokenRequest()
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Audience = options.Audience,
            }
        );
        var earlySeconds = 60;
        await _db.StringSetAsync(
            AccessTokenKey,
            response.AccessToken,
            TimeSpan.FromSeconds(response.ExpiresIn - earlySeconds)
        );

        return response.AccessToken;
    }

    public async Task AssignRole(string id, string[] roles)
    {
        var client = await GetClient();

        await client.Users.AssignRolesAsync(id, new AssignRolesRequest() { Roles = ["admin"] });
    }

    private async Task<ManagementApiClient> GetClient()
    {
        var options = configuration.BindOptions<Auth0Options>();
        var token = await GetAccessToken();
        return new ManagementApiClient(token, new Uri(options.ApiUrl));
    }
}
