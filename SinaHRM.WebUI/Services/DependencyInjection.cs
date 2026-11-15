
using SinaHRM.WebUI.Services.ContractManagement;
using SinaHRM.WebUI.Services.Interfaces;
using SinaHRM.WebUI.Services.Utilities;

namespace SinaHRM.WebUI.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["ApiSettings:BaseUrl"];

        if (!Uri.TryCreate(
                baseUrl,
                UriKind.Absolute,
                out var baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttps && baseUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException(
                "ApiSettings:BaseUrl must be a valid absolute HTTP or HTTPS URL.");
        }

        services.AddHttpClient<IContractManagementApiClient, ContractApiClient>(client =>
        {
            client.BaseAddress = baseUri;
        });
        //services.AddSingleton<IHtmlTemplateService, HtmlTemplateService>();

        return services;
    }
}
