using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using StallosDotnetPleno.Application.Interfaces;

namespace StallosDotnetPleno.Api.Entities;

public class RoosterAuthService : IRoosterAuthService
{
    private readonly HttpClient _httpClient;
    private string _bearerToken = string.Empty;

    public RoosterAuthService() => _httpClient = new HttpClient();

    public async Task LoginAuth(string username, string password)
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
            }

        }
    }

    public string GetBearerToken()
    {
        return _bearerToken;
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }

}