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

public class PessoaService(ApplicationDbContext context, IValidationService validationService, IBackgroundTaskQueue taskQueue, IVerificacaoListaPublicaService verificacaoListaPublica, ILogger<PessoaService> logger, IServiceScopeFactory scopeFactory) : IPessoaService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IValidationService _validationService = validationService;
    private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
    private readonly IVerificacaoListaPublicaService _verificacaoListaPublica = verificacaoListaPublica;
    private readonly ILogger<PessoaService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    public async Task<OperationResult<int>> CreatePessoaAsync(PostPessoaView postPessoaView)
    {
        TipoPessoa tipoPessoa = await GetTipoPessoaAsync(postPessoaView.TipoPessoa);
        if (tipoPessoa == null)
        {
            return OperationResult<int>.FailureResult("TipoPessoa inválido.");
        }

        Pessoa pessoa = await MapToPessoaAsync(postPessoaView, tipoPessoa);

        OperationResult validationResult = ValidatePessoa(pessoa);
        if (!validationResult.Success)
        {
            return OperationResult<int>.FailureResult(validationResult.ErrorMessage);
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


        return OperationResult<int>.SuccessResult(pessoa.Id);
    }

    public async Task<OperationResult<PessoaView>> GetPessoaAsync(int id)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa == null)
        {
            return OperationResult<PessoaView>.FailureResult("Pessoa não encontrada.");
        }

        return OperationResult<PessoaView>.SuccessResult(MapToPessoaView(pessoa));
    }

    public async Task<OperationResult<List<PessoaView>>> GetPessoasAsync()
    {
        var pessoas = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .ToListAsync();

        return OperationResult<List<PessoaView>>.SuccessResult(pessoas.Select(p => MapToPessoaView(p)).ToList());
    }

    public async Task<OperationResult> UpdatePessoaAsync(int id, PostPessoaView postPessoaView)
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

        var validationResult = pessoa.Documento != postPessoaView.Documento
            ? ValidatePessoa(pessoa)
            : ValidatePessoaPut(pessoa);

        if (!validationResult.Success)
        {
            return OperationResult.FailureResult(validationResult.ErrorMessage);
        }

        var tipoPessoa = await GetTipoPessoaAsync(postPessoaView.TipoPessoa);
        if (tipoPessoa == null)
        {
            return OperationResult.FailureResult("TipoPessoa inválido.");
        }

        await UpdatePessoaFromViewAsync(pessoa, postPessoaView, tipoPessoa);

        _context.Pessoas.Update(pessoa);
        await _context.SaveChangesAsync();

        return OperationResult.SuccessResult();
    }

    public async Task<OperationResult> DeletePessoaAsync(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa == null)
        {
            return OperationResult.FailureResult("Pessoa não encontrada.");
        }

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

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
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var verificacaoListaPublica = scope.ServiceProvider.GetRequiredService<IVerificacaoListaPublicaService>();

            List<PessoaLista> listaPublicaPessoa = await verificacaoListaPublica.VerificarListaPublica(pessoa);
            if (listaPublicaPessoa.Count > 0)
            {
                await context.PessoaListas.AddRangeAsync(listaPublicaPessoa, token);
                await context.SaveChangesAsync(token);
            }
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

    private PessoaView MapToPessoaView(Pessoa pessoa)
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

        pessoa.PessoaEnderecos.Clear();
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
    }
}
