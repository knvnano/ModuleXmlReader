using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleValidationService
{
    ValidatedModule Validate(ModuleEditorState state);
}
