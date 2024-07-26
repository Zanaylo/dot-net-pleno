using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using StallosDotnetPleno.Application.Interfaces;

namespace StallosDotnetPleno.Api.Entities;

public class RosterAuthService : IRosterAuthService
{
    private readonly HttpClient _httpClient;
    private string _bearerToken = string.Empty;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public RosterAuthService() => _httpClient = new HttpClient();

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
            var isAuthenticated = await LoginAuth("20jv8p2v8nbl6dn7rrcet4bidd", "1js72l6hr1hl709u2sk56aj0mthb047irvfrna27b98d8o126q27");
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