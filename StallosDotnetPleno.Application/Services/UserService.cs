using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StallosDotnetPleno.Application.Interfaces;

namespace StallosDotnetPleno.Application.Services;

public class UserService : IUserService
{
    private readonly IDictionary<string, string> _users;

    public UserService()
    {
        _users = new Dictionary<string, string>
        {
            { "StallosMaster", "StallosPassword" }
        };
    }

    public bool ValidateUser(string username, string password)
    {
        return _users.TryGetValue(username, out var storedPassword) && storedPassword == password;
    }
}