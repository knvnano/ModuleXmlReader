namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleSerializer
{
    IModuleDocument Deserialize(string path);
    void Serialize(IModuleDocument document, string path);
}
