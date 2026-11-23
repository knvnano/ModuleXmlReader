namespace ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
public interface IModuleEditorService
{
    ModuleEditorState State { get; }
    bool Open(string filePath);
    bool Save(string filePath);
    void UpdateTables(string points, string newScan, string scan);
    void Rotate(Rotation rotation);
}
