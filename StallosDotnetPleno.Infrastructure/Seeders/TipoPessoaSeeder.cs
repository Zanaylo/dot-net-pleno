using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Infrastructure.Seeders;

internal class TipoPessoaSeeder(ApplicationDbContext context) : ITipoPessoaSeeder
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
        }
    }

    private IEnumerable<TipoPessoa> GetTipos()
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
}
