namespace StallosDotnetPleno.Application.Interfaces;

public interface IRoosterAuthService
{
    string GetBearerToken();
    Task LoginAuth(string username, string password);
}