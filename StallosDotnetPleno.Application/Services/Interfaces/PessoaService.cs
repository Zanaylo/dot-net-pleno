using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Application.Services.Interfaces;

public class PessoaService : IPessoaService
{
    private readonly ApplicationDbContext _context;
    private readonly IValidationService _validationService;
    //private readonly IBackgroundTaskQueue _taskQueue;

    public PessoaService(ApplicationDbContext context, IValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
        //_taskQueue = taskQueue;
    }

    public async Task<bool> AddPessoaAsync(Pessoa pessoa)
    {
        // Validate CPF or CNPJ
        if (pessoa.TipoPessoa.Tipo == "PF" && !_validationService.ValidateCPF(pessoa.Documento))
            throw new ArgumentException("Invalid CPF.");
        if (pessoa.TipoPessoa.Tipo == "PJ" && !_validationService.ValidateCNPJ(pessoa.Documento))
            throw new ArgumentException("Invalid CNPJ.");

        // Check if person already exists
        if (_context.Pessoas.Any(p => p.Documento == pessoa.Documento))
            throw new InvalidOperationException("Person already exists.");

        // Ensure at least one address is provided
        if (!pessoa.PessoaEnderecos.Any())
            throw new InvalidOperationException("At least one address must be provided.");

        // Add person to the database
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        // Queue background task to check public list
        //_taskQueue.QueueBackgroundWorkItem(async token =>
        //{
        //    await CheckPublicListAsync(pessoa);
        //});

        return true;
    }

    private async Task CheckPublicListAsync(Pessoa pessoa)
    {
        // Implement the logic to check public list and update TB_PESSOA_LISTA
        bool isInPublicList = await CheckIfPersonIsInPublicList(pessoa);
        if (isInPublicList)
        {
            var pessoaLista = new PessoaLista
            {
                IdPessoa = pessoa.Id,
                Lista = "Public List Name"
            };
            _context.PessoaListas.Add(pessoaLista);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<bool> CheckIfPersonIsInPublicList(Pessoa pessoa)
    {
        // Call the external API and check if the person is in the public list
        // Return true if the person is in the list, otherwise false
        return false; // Placeholder for actual implementation
    }
}
