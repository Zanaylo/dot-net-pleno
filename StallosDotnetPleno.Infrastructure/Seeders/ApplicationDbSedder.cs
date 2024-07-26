using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;
using StallosDotnetPleno.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Infrastructure.Seeders;

internal class ApplicationDbSedder(ApplicationDbContext context) : IApplicationDbSedder
{

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

                await context.Users.AddRangeAsync(users);

                await context.SaveChangesAsync();

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
        List<User> users =
            [
                new(){
                    ClientId = "StallosMaster",
                    ClientSecret = "StallosPassword",
                    RosterId = "20jv8p2v8nbl6dn7rrcet4bidd",
                    RosterSecret = "1js72l6hr1hl709u2sk56aj0mthb047irvfrna27b98d8o126q27",
                    RosterXApi = "Q94j9LQyma446FhErixWe5RzWDtSWKu65HIole5b",
                }

            ];

        return users;
    }
}
