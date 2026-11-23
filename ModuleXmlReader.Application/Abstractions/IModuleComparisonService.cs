using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleComparisonService
{
 IReadOnlyList<ModuleDifference> CompareFile(ModuleEditorState current,string path);
 IReadOnlyList<ModuleDifference> Compare(ModuleEditorState before,ModuleEditorState after);
}
