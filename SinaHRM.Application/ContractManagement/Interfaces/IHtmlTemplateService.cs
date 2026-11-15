namespace SinaHRM.Application.ContractManagement.Interfaces;

public interface IHtmlTemplateService
{
    string Render(string html, Dictionary<string, string> values);
}
