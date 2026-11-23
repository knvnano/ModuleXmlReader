using System.Globalization;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

public sealed class ModuleValidationService(IPointTableParser parser, IScanCodeService scan) : IModuleValidationService
{
    public ValidatedModule Validate(ModuleEditorState state)
    {
        int cols = Positive(state.Parameters["PixelCols"], "PixelCols");
        int rows = Positive(state.Parameters["PixelRows"], "PixelRows");
        int groups = Positive(state.Parameters["DataGroup"], "DataGroup");
        if (groups > 8) throw Messages.Format("GroupRange", "DataGroup: поддерживается от 1 до 8 групп.");
        bool vertical = state.Parameters["DataDirectType"] switch
        {
            "Horizontal" => false, "Vertical" => true,
            _ => throw Messages.Format("DirectionInvalid", "DataDirectType: ожидается Horizontal или Vertical.")
        };
        if ((vertical ? cols : rows) % groups != 0)
            throw Messages.Format("GroupDivision", "Размер вдоль групп данных должен делиться на DataGroup без остатка.");
        var geometry = new ModuleGeometry(cols, rows, groups, vertical);
        if (geometry.GroupWidth > 255 || geometry.GroupHeight > 255)
            throw Messages.Format("GroupSize", "Размер группы превышает диапазон координат таблицы (255).");
        var points = parser.Parse(state.PointTableData);
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            if (!p.IsMissing && (p.X >= geometry.GroupWidth || p.Y >= geometry.GroupHeight))
                throw Messages.Format("PointBounds", $"PointTableData: пара {i + 1} ({p.X}, {p.Y}) выходит за границы группы {geometry.GroupWidth}×{geometry.GroupHeight}." , i + 1, p.X, p.Y, geometry.GroupWidth, geometry.GroupHeight);
        }
        var total = state.Parameters["TotalPointInTable"];
        if (total.Length > 0 && Positive(total, "TotalPointInTable") != points.Count)
            throw Messages.Format("PointCount", $"TotalPointInTable: ожидается {total} пар, получено {points.Count}." , total, points.Count);
        var legacyScan = scan.Parse(state.ScanABCDCode, "ScanABCDCode");
        var newScan = scan.Parse(state.NewScanABCDCode, "NewScanABCDCode");
        string scanType = state.Parameters["ScanType"];
        if (newScan.Length > 0 && scanType.StartsWith("Scan_", StringComparison.Ordinal) &&
            int.TryParse(scanType.AsSpan(5), out int active) && active > 0 && newScan.Length < active)
            throw Messages.Format("NewScanShort", $"NewScanABCDCode: требуется не менее {active} значений." , active);
        int scanLength = scanType.StartsWith("Scan_", StringComparison.Ordinal) &&
            int.TryParse(scanType.AsSpan(5), out int n) && n > 0 ? n : 0;
        var addresses = newScan.Length > 0 ? newScan : legacyScan;
        var activeAddresses = addresses.Take(scanLength > 0 ? Math.Min(scanLength, addresses.Length) : addresses.Length).ToArray();
        return new(geometry, points, Array.AsReadOnly(activeAddresses));
    }
    private static int Positive(string text, string name) => int.TryParse(text, NumberStyles.None,
        CultureInfo.InvariantCulture, out var value) && value > 0 ? value :
        throw Messages.Format("PositiveInteger", $"{name}: требуется положительное целое число." , name);
}
