using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;

public interface IModuleRepository
{
    IModuleDocument Load(string path);
    void Save(IModuleDocument document, string path);
}
