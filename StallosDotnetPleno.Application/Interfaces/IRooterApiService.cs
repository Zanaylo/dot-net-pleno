namespace StallosDotnetPleno.Application.Interfaces;

public interface IRooterApiService
{
    Task<string> BolsaFamilia(string name, string cpf, string authorizationToken);
    Task<string> Cepim(string name, string authorizationToken);
    Task<string> GetProtocol(string bearerToken);
    Task<string> Interpol(string name, string cpf, string authorizationToken);
    Task<string> Ofac(string name, string authorizationToken);
    Task<string> Pep(string name, string cpf, string authorizationToken);
}