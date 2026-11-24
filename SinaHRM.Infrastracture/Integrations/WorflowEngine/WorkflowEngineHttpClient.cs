using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SinaHRM.Application.Integrations.WorkflowEngine.Contracts;
using SinaHRM.Application.Integrations.WorkflowEngine.Interfaces;

namespace SinaHRM.Infrastracture.Data.Sql.Integrations.WorkflowEngine;

public sealed class WorkflowEngineHttpClient : IWorkflowEngineGateway
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public WorkflowEngineHttpClient(
        HttpClient httpClient,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _environment = environment;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<Guid> StartAsync(
        StartContractWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var baseUrl = _configuration["WorkflowIntegration:WorkflowEngine:BaseUrl"];
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttps && !_environment.IsDevelopment()))
        {
            throw new InvalidOperationException(
                "A valid HTTPS WorkflowIntegration:WorkflowEngine:BaseUrl must be configured.");
        }

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(baseUri, "/api/v1/workflows/start"));

        if (_environment.IsDevelopment())
        {
            var developmentToken = _configuration["Authentication:DevelopmentBearerToken"];
            if (string.IsNullOrWhiteSpace(developmentToken))
            {
                throw new InvalidOperationException(
                    "Authentication:DevelopmentBearerToken must be configured for Development WorkflowEngine calls.");
            }

            message.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", developmentToken);
        }
        else
        {
            throw new NotSupportedException(
                "Production WorkflowEngine service authentication is not configured. Implement the approved service-to-service JWT/client-credentials flow before enabling this integration.");
        }

        message.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<WorkflowStartResponse>(
            cancellationToken: cancellationToken);

        if (result is null || result.Id == Guid.Empty)
            throw new InvalidOperationException("WorkflowEngine returned an invalid start response.");

        return result.Id;
    }
}
