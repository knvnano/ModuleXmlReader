using System.Globalization;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

public sealed class PointTableParser : IPointTableParser
{
    public IReadOnlyList<PointTableEntry> Parse(string pointTableData)
    {
        if (string.IsNullOrWhiteSpace(pointTableData)) throw Messages.Format("PointsEmpty", "PointTableData: таблица пуста.");
        var tokens = pointTableData.Split('-');
        if (tokens.Length % 2 != 0) throw Messages.Format("PointsPairs", "PointTableData: требуется чётное число значений (пары X-Y).");
        var points = new PointTableEntry[tokens.Length / 2];
        for (int i = 0; i < points.Length; i++)
        {
            int x = Read(tokens[i * 2], i * 2), y = Read(tokens[i * 2 + 1], i * 2 + 1);
            if ((x == 255) != (y == 255)) throw Messages.Format("PointsMarker", $"PointTableData: неполный маркер 255-255 в паре {i + 1}." , i + 1);
            points[i] = new(x, y);
        }
        return Array.AsReadOnly(points);
    }
    private static int Read(string text, int index) =>
        byte.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var value)
            ? value : throw Messages.Format("PointByte", $"PointTableData: значение {index + 1} должно быть целым числом от 0 до 255." , index + 1);
    public string Format(IEnumerable<PointTableEntry> points) => string.Join("-", points.SelectMany(p => new[] { p.X, p.Y }));
}
