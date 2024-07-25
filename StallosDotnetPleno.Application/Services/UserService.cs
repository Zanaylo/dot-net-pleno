using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StallosDotnetPleno.Application.Interfaces;

namespace StallosDotnetPleno.Application.Services;

public class UserService : IUserService
{
    public bool ValidateUser(string username, string password)
    {
        return username == "StallosMaster" && password == "StallosPassword";
    }
}
