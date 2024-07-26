using StallosDotnetPleno.Application.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StallosDotnetPleno.Api.Entities;

public class RosterApiService : IRosterApiService
{
    private readonly Dictionary<string, string> _protocolCache = new Dictionary<string, string>();

    private async Task<string> GetOrCreateProtocol(string listType, string bearerToken)
    {
        if (!_protocolCache.TryGetValue(listType, out var protocol) || string.IsNullOrEmpty(protocol))
        {
            protocol = await GetProtocol(bearerToken, listType);
            _protocolCache[listType] = protocol;
        }
        return protocol;
    }

    public async Task<string> ConsultaBolsaFamilia(string name, string cpf, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetOrCreateProtocol("bolsa-familia", authorizationToken);
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

    public async Task<string> ConsultaPep(string name, string cpf, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetOrCreateProtocol("pep", authorizationToken);
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

    public async Task<string> ConsultaInterpol(string name, string cpf, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetOrCreateProtocol("interpol", authorizationToken);
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

    public async Task<string> ConsultaCepim(string name, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetOrCreateProtocol("cepim", authorizationToken);
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

    public async Task<string> ConsultaOfac(string name, string authorizationToken)
    {
        var client = new HttpClient();
        var protocolo = await GetOrCreateProtocol("ofac", authorizationToken);
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

    private async Task<string> GetProtocol(string bearerToken, string listType)
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
            Listas = new string[] { listType }
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