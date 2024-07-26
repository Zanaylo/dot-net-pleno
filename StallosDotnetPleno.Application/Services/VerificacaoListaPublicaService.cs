using Microsoft.Extensions.DependencyInjection;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;

namespace StallosDotnetPleno.Api.Entities;

public class VerificacaoListaPublicaService : IVerificacaoListaPublicaService
{
    private readonly IRosterAuthService _authService;
    private readonly IRosterApiService _rooterApi;
    private readonly IUserService _userService;

    public VerificacaoListaPublicaService(IRosterAuthService authService, IRosterApiService rooterApi, IUserService userService)
    {
        _authService = authService;
        _rooterApi = rooterApi;
        _userService = userService;
    }

    public async Task<List<PessoaLista>> VerificarListaPublica(Pessoa pessoa)
    {
        await _authService.EnsureTokenAsync();
        var bearerToken = _authService.GetBearerToken();

        if (string.IsNullOrEmpty(bearerToken))
        {
            return pessoa.PessoaListas;
        }

        pessoa.PessoaListas = new();

        if (pessoa.TipoPessoa.Tipo.ToUpper() == "PF")
        {
            var bolsaFamiliaResponse = await _rooterApi.ConsultaBolsaFamilia(pessoa.Nome, pessoa.Documento, bearerToken);

            if (!string.IsNullOrEmpty(bolsaFamiliaResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = bolsaFamiliaResponse });
            }
            var pepResponse = await _rooterApi.ConsultaPep(pessoa.Nome, pessoa.Documento, bearerToken);

            if (!string.IsNullOrEmpty(pepResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = pepResponse });
            }

            var interpolResponse = await _rooterApi.ConsultaInterpol(pessoa.Nome, pessoa.Documento, bearerToken);

            if (!string.IsNullOrEmpty(interpolResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = interpolResponse });
            }

        }
        else if (pessoa.TipoPessoa.Tipo.ToUpper() == "PJ")
        {
            var cepimResponse = await _rooterApi.ConsultaCepim(pessoa.Nome, bearerToken);
            if (!string.IsNullOrEmpty(cepimResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = cepimResponse });
            }

            var ofacResponse = await _rooterApi.ConsultaOfac(pessoa.Nome, bearerToken);
            if (!string.IsNullOrEmpty(ofacResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = ofacResponse });
            }
        }

        return pessoa.PessoaListas;
    }
}