using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IPreviewService
{
    PreviewData Create(ValidatedModule module);
}
