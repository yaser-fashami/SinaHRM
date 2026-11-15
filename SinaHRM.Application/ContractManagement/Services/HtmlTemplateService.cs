using SinaHRM.Application.ContractManagement.Interfaces;
using System.Net;
using System.Text.RegularExpressions;

namespace SinaHRM.Application.ContractManagement.Services;

public sealed class HtmlTemplateService : IHtmlTemplateService
{
    private static readonly Regex PlaceholderRegex =
        new(@"\{\{(.*?)\}\}", RegexOptions.Compiled);

    public string Render(
        string html,
        Dictionary<string, string> values)
    {
        return PlaceholderRegex.Replace(html, match =>
        {
            var key = match.Groups[1].Value.Trim();

            if (!values.TryGetValue(key, out var value))
                return match.Value;

            return WebUtility.HtmlEncode(value ?? string.Empty);
        });
    }
}
