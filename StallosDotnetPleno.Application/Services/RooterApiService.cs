using StallosDotnetPleno.Application.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StallosDotnetPleno.Api.Entities;

public class RooterApiService(IRoosterAuthService authService) : IRooterApiService
{
    public async Task<string> BolsaFamilia(string name, string cpf, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetProtocol(authorizationToken);
        //var requestUri = $"https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/bolsa-familia?nome={Uri.EscapeDataString(name)}";
        var requestUri = $"https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/bolsa-familia?nome={Uri.EscapeDataString(name)}&cpf={Uri.EscapeDataString(cpf)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("protocolo", protocolo);
        request.Headers.Add("Authorization", authorizationToken);

        try
        {
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "bolsa-familia";
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public async Task<string> Pep(string name, string cpf, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetProtocol(authorizationToken);
        var requestUri = $"https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/pep?nome={Uri.EscapeDataString(name)}&cpf={Uri.EscapeDataString(cpf)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("protocolo", protocolo);
        request.Headers.Add("Authorization", authorizationToken);

        try
        {
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "pep";
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public async Task<string> Interpol(string name, string cpf, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetProtocol(authorizationToken);
        var requestUri = $"https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/interpol?nome={Uri.EscapeDataString(name)}&cpf={Uri.EscapeDataString(cpf)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("protocolo", protocolo);
        request.Headers.Add("Authorization", authorizationToken);

        try
        {
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "interpol";
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public async Task<string> Cepim(string name, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetProtocol(authorizationToken);
        var requestUri = $"https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/cepim?nome={Uri.EscapeDataString(name)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("protocolo", protocolo);
        request.Headers.Add("Authorization", authorizationToken);

        try
        {
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "cepim";
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public async Task<string> Ofac(string name, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetProtocol(authorizationToken);
        var requestUri = $"https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/ofac?nome={Uri.EscapeDataString(name)}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("protocolo", protocolo);
        request.Headers.Add("Authorization", authorizationToken);

        try
        {
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return "ofac";
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public async Task<string> GetProtocol(string bearerToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://x5hn0kjhpl.execute-api.us-east-2.amazonaws.com/prd/roster/v2/protocolo");
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("x-api-key", "Q94j9LQyma446FhErixWe5RzWDtSWKu65HIole5b");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var requestBody = new
        {
            Responsavel = "XPTO",
            Origem = 2,
            Consulta = new
            {
                Nome = "XPTO",
                Documento = "00000000000"
            },
            Listas = new string[]
            {
                "bolsa-familia",
                "cnep",
                "cepim",
                "ceis",
                "ceaf",
                "csnu",
                "auxilio-emergencial",
                "pep",
                "garantia-safra",
                "peti",
                "ofac",
                "seguro-defeso",
                "inabilitados-bacen",
                "interpol",
                "ibama",
                "tcu",
                "fbi",
                "combate-escravidao"
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        request.Content = content;

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var protocoloResponse = JsonSerializer.Deserialize<ProtocoloResponse>(responseBody);

            var protocolo = protocoloResponse!.Protocolo;

            return protocolo;
        }
        else
        {

            return string.Empty;
        }

    }

    private class ProtocoloResponse
    {
        public string Protocolo { get; set; }
        public string Message { get; set; }
    }

}