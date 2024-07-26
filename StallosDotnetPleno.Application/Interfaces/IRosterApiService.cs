namespace StallosDotnetPleno.Application.Interfaces;

public interface IRosterApiService
{
    Task<string> ConsultaBolsaFamilia(string name, string cpf, string authorizationToken);
    Task<string> ConsultaCepim(string name, string authorizationToken);
    Task<string> ConsultaInterpol(string name, string cpf, string authorizationToken);
    Task<string> ConsultaOfac(string name, string authorizationToken);
    Task<string> ConsultaPep(string name, string cpf, string authorizationToken);
}