using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;

namespace StallosDotnetPleno.Application.Interfaces
{
    public interface IUserService
    {
        User GetUserByClientId(string clientId);
        User ReturnUser();
        bool ValidateUser(string clientId, string clientSecret);
    }
}