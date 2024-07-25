using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Domain.ViewModels;
using StallosDotnetPleno.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Application.Services;

public class PessoaService : IPessoaService
{
    private readonly ApplicationDbContext _context;
    private readonly IValidationService _validationService;
    private readonly IBackgroundTaskQueue _taskQueue;

    public PessoaService(ApplicationDbContext context, IValidationService validationService, IBackgroundTaskQueue taskQueue)
    {
        _context = context;
        _validationService = validationService;
        _taskQueue = taskQueue;
    }

    public async Task<int> CreatePessoaAsync(PostPessoaView postPessoaView)
    {
        var tipoPessoa = await GetTipoPessoaAsync(postPessoaView.TipoPessoa);
        if (tipoPessoa == null)
        {
            throw new ArgumentException("TipoPessoa inválido.");
        }

        var pessoa = await MapToPessoaAsync(postPessoaView, tipoPessoa);

        ValidatePessoa(pessoa);

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            await CheckPublicListAsync(pessoa);
        });

        return pessoa.Id;
    }

    public async Task<PessoaView> GetPessoaAsync(int id)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        return MapToPessoaView(pessoa);
    }

    public async Task<List<PessoaView>> GetPessoasAsync()
    {
        var pessoas = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .ToListAsync();

        return pessoas.Select(p => MapToPessoaView(p)).ToList();
    }

    public async Task UpdatePessoaAsync(int id, PostPessoaView postPessoaView)
    {
        var pessoa = await _context.Pessoas
            .Include(p => p.TipoPessoa)
            .Include(p => p.PessoaEnderecos).ThenInclude(pe => pe.Endereco)
            .Include(p => p.PessoaListas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pessoa == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }


        if (pessoa.Documento != postPessoaView.Documento)
        {
            ValidatePessoa(pessoa);
        }
        else
        {
            ValidatePessoaPut(pessoa);
        }

        var tipoPessoa = await GetTipoPessoaAsync(postPessoaView.TipoPessoa);
        if (tipoPessoa == null)
        {
            throw new ArgumentException("TipoPessoa inválido.");
        }

        await UpdatePessoaFromViewAsync(pessoa, postPessoaView, tipoPessoa);


        _context.Pessoas.Update(pessoa);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePessoaAsync(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();
    }

    private void ValidatePessoa(Pessoa pessoa)
    {
        var tipoPessoa = pessoa.TipoPessoa.Tipo.ToUpper();

        if (tipoPessoa == "PF" && !_validationService.ValidateCPF(pessoa.Documento))
            throw new ArgumentException("CPF INVÁLIDO.");
        if (tipoPessoa == "PJ" && !_validationService.ValidateCNPJ(pessoa.Documento))
            throw new ArgumentException("CNPJ INVÁLIDO.");

        if (_context.Pessoas.Any(p => p.Documento == pessoa.Documento))
            throw new InvalidOperationException($"{(tipoPessoa == "PF" ? "CPF" : "CNPJ")} já cadastrado");

        if (!pessoa.PessoaEnderecos.Any())
            throw new InvalidOperationException("Pelo menos um endereço precisa ser cadastrado.");
    }

    private void ValidatePessoaPut(Pessoa pessoa)
    {
        var tipoPessoa = pessoa.TipoPessoa.Tipo.ToUpper();

        if (tipoPessoa == "PF" && !_validationService.ValidateCPF(pessoa.Documento))
            throw new ArgumentException("CPF INVÁLIDO.");
        if (tipoPessoa == "PJ" && !_validationService.ValidateCNPJ(pessoa.Documento))
            throw new ArgumentException("CNPJ INVÁLIDO.");

        if (!pessoa.PessoaEnderecos.Any())
            throw new InvalidOperationException("Pelo menos um endereço precisa ser cadastrado.");
    }

    private async Task CheckPublicListAsync(Pessoa pessoa)
    {
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
        
        return false;
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
        var pessoa = new Pessoa
        {
            Nome = postPessoaView.Nome,
            Documento = postPessoaView.Documento,
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
