using Microsoft.Extensions.Configuration;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;
using StallosDotnetPleno.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Infrastructure.Seeders;

internal class ApplicationDbSedder(ApplicationDbContext context, IConfiguration configuration) : IApplicationDbSedder
{
    private readonly IConfiguration _configuration = configuration;

    public async Task Seed()
    {
        if (await context.Database.CanConnectAsync())
        {
            if (!context.TipoPessoas.Any())
            {
                var TipoPessoas = GetTipos();

                await context.TipoPessoas.AddRangeAsync(TipoPessoas);

                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var users = CreateUsers();

                if (users.Count > 0)
                {
                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    private List<TipoPessoa> GetTipos()
    {
        List<TipoPessoa> tipoPessoas =
            [
                new(){
                    Tipo = "PF"
                },
                new(){
                    Tipo = "PJ"
                }
            ];


        return tipoPessoas;
    }

    private List<User> CreateUsers()
    {
        string clientId = _configuration["UserCredentials:ClientId"]!;
        string clientSecret = _configuration["UserCredentials:ClientSecret"]!;
        string rosterId = _configuration["UserCredentials:RosterId"]!;
        string rosterSecret = _configuration["UserCredentials:RosterSecret"]!;
        string rosterXApi = _configuration["UserCredentials:RosterXApi"]!;

        List<User> users = new List<User>();
        if (!string.IsNullOrEmpty(clientId) &&
            !string.IsNullOrEmpty(clientSecret) &&
            !string.IsNullOrEmpty(rosterId) &&
            !string.IsNullOrEmpty(rosterSecret) &&
            !string.IsNullOrEmpty(rosterXApi))
        {
            users.Add(new User
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                RosterId = rosterId,
                RosterSecret = rosterSecret,
                RosterXApi = rosterXApi,
            });
        }

        return users;
    }
}
