using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Application.ResultObject;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Domain.ViewModels;
using StallosDotnetPleno.Infrastructure.Context;
using StallosDotnetPleno.Infrastructure.Migrations;
using System.Text.RegularExpressions;

namespace StallosDotnetPleno.Application.Services;

public class PessoaService(ApplicationDbContext context, IValidationService validationService, IBackgroundTaskQueue taskQueue, ILogger<PessoaService> logger, IServiceScopeFactory scopeFactory) : IPessoaService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IValidationService _validationService = validationService;
    private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
    private readonly ILogger<PessoaService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    public async Task<OperationResult> CreatePessoaAsync(PostPessoaView postPessoaView)
    {
        TipoPessoa tipoPessoa = await GetTipoPessoaAsync(postPessoaView.TipoPessoa);
        if (tipoPessoa == null)
        {
            return OperationResult.FailureResult("TipoPessoa inválido.");
        }

        Pessoa pessoa = await MapToPessoaAsync(postPessoaView, tipoPessoa);

        OperationResult validationResult = ValidatePessoa(pessoa);
        if (!validationResult.Success)
        {
            return OperationResult.FailureResult(validationResult.ErrorMessage);
        }

        await _context.Pessoas.AddAsync(pessoa);
        await _context.SaveChangesAsync();

        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            try
            {
                await CheckPublicListAsync(pessoa, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar na lista.");
            }
        });


        return OperationResult.SuccessResult(pessoa.Id);
    }

    public async Task<OperationResult> GetPessoaAsync(int id)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa == null)
        {
            return OperationResult.FailureResult("Pessoa não encontrada.");
        }

        return OperationResult.SuccessResult(MapToPessoaView(pessoa));
    }

    public async Task<OperationResult> GetPessoasAsync()
    {
        var pessoas = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .ToListAsync();

        return OperationResult.SuccessResult(pessoas.Select(p => MapToPessoaView(p)).ToList());
    }

    public async Task<OperationResult> UpdatePessoaAsync(int id, PostPessoaView postPessoaView)
    {
        var documentCorrigido = Regex.Replace(postPessoaView.Documento, @"[^\d]", "");

        var cadastroPessoa = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (cadastroPessoa == null)
        {
            return OperationResult.FailureResult("Pessoa não encontrada.");
        }

        TipoPessoa tipoPessoa = await GetTipoPessoaAsync(postPessoaView.TipoPessoa);
        if (tipoPessoa == null)
        {
            return OperationResult.FailureResult("TipoPessoa inválido.");
        }

        var existingPessoa = await _context.Pessoas
            .FirstOrDefaultAsync(p => p.Documento == documentCorrigido && p.Id != id);
        if (existingPessoa != null)
        {
            return OperationResult.FailureResult("Documento já cadastrado para outra pessoa.");
        }

        var documentChanged = cadastroPessoa.Documento != documentCorrigido;
        var tipoPessoaChanged = cadastroPessoa.TipoPessoa != tipoPessoa;
        var nomeChanged = cadastroPessoa.Nome != postPessoaView.Nome;

        if (tipoPessoaChanged && !documentChanged)
        {
            return OperationResult.FailureResult("Tipo Pessoa só poderá ser alterado se o documento também ser alterado.");
        }

        postPessoaView.Documento = documentCorrigido;
        await UpdatePessoaFromViewAsync(cadastroPessoa, postPessoaView, tipoPessoa);

        var oldEnderecos = cadastroPessoa.PessoaEnderecos.Select(pe => pe.Endereco).ToList();
        var newEnderecos = cadastroPessoa.PessoaEnderecos.Select(pe => pe.Endereco).ToList();
        var removedEnderecos = oldEnderecos.Except(newEnderecos).ToList();

        _context.Pessoas.Update(cadastroPessoa);
        await _context.SaveChangesAsync();

        var unusedEnderecos = await _context.Enderecos
            .Where(e => removedEnderecos.Select(re => re.Id).Contains(e.Id) &&
                        !_context.PessoaEnderecos.Any(pe => pe.IdEndereco == e.Id))
            .ToListAsync();

        if (unusedEnderecos.Count != 0)
        {
            _context.Enderecos.RemoveRange(unusedEnderecos);
            await _context.SaveChangesAsync();
        }

        if (documentChanged || nomeChanged)
        {
            cadastroPessoa.PessoaListas.Clear();
            await _context.SaveChangesAsync();
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    await CheckPublicListAsync(cadastroPessoa, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar na lista.");
                }
            });
        }

        return OperationResult.SuccessResult();
    }


    public async Task<OperationResult> DeletePessoaAsync(int id)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.PessoaEnderecos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa == null)
        {
            return OperationResult.FailureResult("Pessoa não encontrada.");
        }

        var enderecosId = pessoa.PessoaEnderecos.Select(pe => pe.IdEndereco).ToList();

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        var orpahnEnderecos = await _context.Enderecos
            .Where(e => enderecosId.Contains(e.Id) && !_context.PessoaEnderecos.Any(pe => pe.IdEndereco == e.Id))
            .ToListAsync();

        if (orpahnEnderecos.Count != 0)
        {
            _context.Enderecos.RemoveRange(orpahnEnderecos);
            await _context.SaveChangesAsync();
        }

        return OperationResult.SuccessResult();
    }

    private OperationResult ValidatePessoa(Pessoa pessoa)
    {
        var tipoPessoa = pessoa.TipoPessoa.Tipo.ToUpper();

        if (tipoPessoa == "PF" && !_validationService.ValidateCPF(pessoa.Documento))
            return OperationResult.FailureResult("CPF INVÁLIDO.");
        if (tipoPessoa == "PJ" && !_validationService.ValidateCNPJ(pessoa.Documento))
            return OperationResult.FailureResult("CNPJ INVÁLIDO.");

        if (_context.Pessoas.Any(p => p.Documento == pessoa.Documento))
            return OperationResult.FailureResult($"{(tipoPessoa == "PF" ? "CPF" : "CNPJ")} já cadastrado");

        if (!pessoa.PessoaEnderecos.Any())
            return OperationResult.FailureResult("Pelo menos um endereço precisa ser cadastrado.");

        return OperationResult.SuccessResult();
    }

    private OperationResult ValidatePessoaPut(Pessoa pessoa)
    {
        var tipoPessoa = pessoa.TipoPessoa.Tipo.ToUpper();

        if (tipoPessoa == "PF" && !_validationService.ValidateCPF(pessoa.Documento))
            return OperationResult.FailureResult("CPF INVÁLIDO.");
        if (tipoPessoa == "PJ" && !_validationService.ValidateCNPJ(pessoa.Documento))
            return OperationResult.FailureResult("CNPJ INVÁLIDO.");

        if (!pessoa.PessoaEnderecos.Any())
            return OperationResult.FailureResult("Pelo menos um endereço precisa ser cadastrado.");

        return OperationResult.SuccessResult();
    }

    public async Task CheckPublicListAsync(Pessoa pessoa, CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var verificacaoListaPublica = scope.ServiceProvider.GetRequiredService<IVerificacaoListaPublicaService>();

        List<PessoaLista> listaPublicaPessoa = await verificacaoListaPublica.VerificarListaPublica(pessoa);
        if (listaPublicaPessoa.Count > 0)
        {
            await context.PessoaListas.AddRangeAsync(listaPublicaPessoa, token);
            await context.SaveChangesAsync(token);
        }
    }

    private async Task<TipoPessoa> GetTipoPessoaAsync(string tipo)
    {
        string tipoPessoa = tipo.ToUpper();
        return await _context.TipoPessoas
            .FirstOrDefaultAsync(tp => tp.Tipo == tipoPessoa);
    }

    private async Task<Endereco> GetExistingEnderecoAsync(EnderecoView enderecoView)
    {
        return await _context.Enderecos
            .FirstOrDefaultAsync(e =>
                e.Cep == enderecoView.Cep &&
                e.Logradouro == enderecoView.Logradouro &&
                e.Numero == enderecoView.Numero &&
                e.Bairro == enderecoView.Bairro &&
                e.Cidade == enderecoView.Cidade &&
                e.Uf == enderecoView.Uf);
    }

    private async Task<Pessoa> MapToPessoaAsync(PostPessoaView postPessoaView, TipoPessoa tipoPessoa)
    {

        var documentCorrigido = Regex.Replace(postPessoaView.Documento, @"[^\d]", "");

        var pessoa = new Pessoa
        {
            Nome = postPessoaView.Nome,
            Documento = documentCorrigido,
            TipoPessoa = tipoPessoa
        };

        foreach (var enderecoView in postPessoaView.Enderecos)
        {
            var endereco = await GetExistingEnderecoAsync(enderecoView);
            if (endereco == null)
            {
                endereco = new Endereco
                {
                    Cep = enderecoView.Cep,
                    Logradouro = enderecoView.Logradouro,
                    Numero = enderecoView.Numero,
                    Bairro = enderecoView.Bairro,
                    Cidade = enderecoView.Cidade,
                    Uf = enderecoView.Uf
                };
            }

            pessoa.PessoaEnderecos.Add(new PessoaEndereco
            {
                Endereco = endereco
            });
        }

        return pessoa;
    }

    private static PessoaView MapToPessoaView(Pessoa pessoa)
    {
        return new PessoaView
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            TipoPessoa = pessoa.TipoPessoa.Tipo,
            Documento = pessoa.Documento,
            Enderecos = pessoa.PessoaEnderecos.Select(pe => new EnderecoView
            {
                Cep = pe.Endereco.Cep,
                Logradouro = pe.Endereco.Logradouro,
                Numero = pe.Endereco.Numero,
                Bairro = pe.Endereco.Bairro,
                Cidade = pe.Endereco.Cidade,
                Uf = pe.Endereco.Uf
            }).ToList(),
            Lista = pessoa.PessoaListas.Select(pl => pl.Lista).ToList()
        };
    }

    private async Task UpdatePessoaFromViewAsync(Pessoa pessoa, PostPessoaView postPessoaView, TipoPessoa tipoPessoa)
    {
        pessoa.Nome = postPessoaView.Nome;
        pessoa.Documento = postPessoaView.Documento;
        pessoa.TipoPessoa = tipoPessoa;

        var newPessoaEnderecos = new List<PessoaEndereco>();

        foreach (var enderecoView in postPessoaView.Enderecos)
        {
            var endereco = await GetExistingEnderecoAsync(enderecoView);
            if (endereco == null)
            {
                endereco = new Endereco
                {
                    Cep = enderecoView.Cep,
                    Logradouro = enderecoView.Logradouro,
                    Numero = enderecoView.Numero,
                    Bairro = enderecoView.Bairro,
                    Cidade = enderecoView.Cidade,
                    Uf = enderecoView.Uf
                };
                _context.Enderecos.Add(endereco);
            }
            newPessoaEnderecos.Add(new PessoaEndereco { Endereco = endereco });
        }

        pessoa.PessoaEnderecos.Clear();
        pessoa.PessoaEnderecos.AddRange(newPessoaEnderecos);
    }
}
