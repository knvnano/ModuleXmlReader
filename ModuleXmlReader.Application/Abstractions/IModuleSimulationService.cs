using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleSimulationService
{
    SimulationModel Create(ValidatedModule module, ModuleEditorState state);
}
