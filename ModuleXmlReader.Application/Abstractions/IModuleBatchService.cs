using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IModuleBatchService
{
 IReadOnlyList<BatchResult> Process(string inputFolder,string? outputFolder,bool variants,CancellationToken cancellation=default);
}
