using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleRotationService
{
    int? DetectOrientation(ModuleEditorState state);
    void Rotate(ModuleEditorState candidate, Rotation rotation);
}
