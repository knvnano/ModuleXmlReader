using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;

public interface IMissingPixelService
{
    string CalculateDisplayText(string pointTableData);
}
