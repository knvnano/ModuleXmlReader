using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Abstractions;
public interface IPointTableParser
{
    IReadOnlyList<PointTableEntry> Parse(string pointTableData);
    string Format(IEnumerable<PointTableEntry> points);
}
