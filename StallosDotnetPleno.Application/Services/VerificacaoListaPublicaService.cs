using Microsoft.Extensions.DependencyInjection;
using StallosDotnetPleno.Application.Interfaces;
using StallosDotnetPleno.Domain.Entities;
using StallosDotnetPleno.Infrastructure.Context;

namespace StallosDotnetPleno.Api.Entities;

public class VerificacaoListaPublicaService : IVerificacaoListaPublicaService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoosterAuthService _authService;
    private readonly IRooterApiService _rooterApi;

    public VerificacaoListaPublicaService(IRoosterAuthService authService, IRooterApiService rooterApi, ApplicationDbContext context)
    {
        _context = context;
        _authService = authService;
        _rooterApi = rooterApi;
    }

    public async Task<List<PessoaLista>> VerificarListaPublica(Pessoa pessoa)
    {
        if (string.IsNullOrWhiteSpace(_authService.GetBearerToken()))
        {
            await _authService.LoginAuth("20jv8p2v8nbl6dn7rrcet4bidd", "1js72l6hr1hl709u2sk56aj0mthb047irvfrna27b98d8o126q27");
        }
        var bearerToken = _authService.GetBearerToken();

        if (string.IsNullOrEmpty(bearerToken))
        {
            return pessoa.PessoaListas;
        }
        pessoa.PessoaListas = new();

        if (pessoa.TipoPessoa.Tipo.ToUpper() == "PF")
        {
            var bolsaFamiliaResponse = await _rooterApi.BolsaFamilia(pessoa.Nome, pessoa.Documento, bearerToken);

            if (!string.IsNullOrEmpty(bolsaFamiliaResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = bolsaFamiliaResponse });
            }
            var pepResponse = await _rooterApi.Pep(pessoa.Nome, pessoa.Documento, bearerToken);

            if (!string.IsNullOrEmpty(pepResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = pepResponse });
            }

            var interpolResponse = await _rooterApi.Interpol(pessoa.Nome, pessoa.Documento, bearerToken);

            if (!string.IsNullOrEmpty(interpolResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = interpolResponse });
            }

        }
        else if (pessoa.TipoPessoa.Tipo.ToUpper() == "PJ")
        {
            var cepimResponse = await _rooterApi.Cepim(pessoa.Nome, bearerToken);
            if (!string.IsNullOrEmpty(cepimResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = cepimResponse });
            }

            var ofacResponse = await _rooterApi.Ofac(pessoa.Nome, bearerToken);
            if (!string.IsNullOrEmpty(ofacResponse))
            {
                pessoa.PessoaListas.Add(new PessoaLista { IdPessoa = pessoa.Id, Lista = ofacResponse });
            }
        }

        return pessoa.PessoaListas;

    }
}
