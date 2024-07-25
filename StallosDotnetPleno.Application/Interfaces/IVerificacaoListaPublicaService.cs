using StallosDotnetPleno.Domain.Entities;

namespace StallosDotnetPleno.Application.Interfaces
{
    public interface IVerificacaoListaPublicaService
    {
        Task<List<PessoaLista>> VerificarListaPublica(Pessoa pessoa);
    }
}