namespace StallosDotnetPleno.Application.Interfaces;

public interface IRosterAuthService
{
    string GetBearerToken();
    Task<bool> LoginAuth(string username, string password);
    Task EnsureTokenAsync();
}