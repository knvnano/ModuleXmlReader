using ModuleXmlReader.Application.Abstractions;
namespace ModuleXmlReader.Application.Services;
public sealed class MissingPixelService(IPointTableParser parser) : IMissingPixelService
{
    public string CalculateDisplayText(string pointTableData)
    {
        var missing = parser.Parse(pointTableData).Select((p, i) => (p, i)).Where(v => v.p.IsMissing).Select(v => v.i + 1).ToArray();
        return missing.Length == 0 ? "Не найдено пропущенных пикселей." : string.Join(", ", missing);
    }
}
