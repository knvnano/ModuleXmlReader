using ModuleXmlReader.Application.Abstractions;
namespace ModuleXmlReader.Infrastructure;

public sealed class XmlModuleRepository(IModuleSerializer serializer) : IModuleRepository
{
    public IModuleDocument Load(string path) => serializer.Deserialize(path);
    public void Save(IModuleDocument document, string path)
    {
        string destination = Path.GetFullPath(path);
        string temp = Path.Combine(Path.GetDirectoryName(destination)!, "." + Path.GetFileName(destination) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            serializer.Serialize(document, temp);
            // Same-directory rename replaces the destination only after serialization succeeds.
            File.Move(temp, destination, overwrite: true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }
}
