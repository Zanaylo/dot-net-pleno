using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Domain.Entities;

namespace StallosDotnetPleno.Api.Entities;

public class RosterAuthService : IRosterAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IUserService _userService;
    private string _bearerToken = string.Empty;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public RosterAuthService(HttpClient httpClient, IUserService userService)
    {
        _httpClient = httpClient;
        _userService = userService;
    }

    public async Task<bool> LoginAuth(string username, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://stallosopendata.auth.us-east-2.amazoncognito.com/oauth2/token");
        request.Headers.Add("accept", "*/*");

        var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var collection = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("scope", "opendata/opendata")
        };

        var content = new FormUrlEncodedContent(collection);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody);

            string bearerToken = tokenResponse.access_token;

            if (!string.IsNullOrEmpty(bearerToken))
            {
                _bearerToken = bearerToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in);
                return true;
            }
        }
        return false;
    }

    public string GetBearerToken()
    {
        if (DateTime.UtcNow >= _tokenExpiration)
        {
            _bearerToken = string.Empty;
        }
        return _bearerToken;
    }

    public async Task EnsureTokenAsync()
    {
        if (string.IsNullOrEmpty(_bearerToken) || DateTime.UtcNow >= _tokenExpiration)
        {
            User credenciais = _userService.ReturnUser();

            var isAuthenticated = await LoginAuth(credenciais.RosterId, credenciais.RosterSecret);
            if (!isAuthenticated)
            {
                throw new UnauthorizedAccessException("Unable to authenticate and retrieve a valid bearer token.");
            }
        }
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}