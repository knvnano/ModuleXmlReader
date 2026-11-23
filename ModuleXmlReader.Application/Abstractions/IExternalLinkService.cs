using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;

public interface IExternalLinkService
{
    void Open(string url);
}
