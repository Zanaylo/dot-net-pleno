using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;

namespace StallosDotnetPleno.Application.Services;

public class UserService(ApplicationDbContext context, IMemoryCache cache) : IUserService
{
    //private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context = context;
    private readonly IMemoryCache _cache = cache;

    //public UserService(IServiceProvider serviceProvider, IMemoryCache cache)
    //{
    //    _serviceProvider = serviceProvider;
    //    _cache = cache;
    //}

    private void SaveUser(User user)
    {
        _cache.Set("LoggedUser", user);
    }

    public bool ValidateUser(string clientId, string clientSecret)
    {
        var user = GetUserByClientId(clientId);

        if (user != null && user.ClientSecret == clientSecret)
        {
            SaveUser(user);
            return true;
        }
        else
        {
            return false;
        }
    }

    public User GetUserByClientId(string clientId)
    {
        return _context.Users.SingleOrDefault(u => u.ClientId == clientId);
    }

    public User ReturnUser()
    {
        _cache.TryGetValue("LoggedUser", out User loggedUser);
        return loggedUser;
    }
}