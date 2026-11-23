using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

// Coordinates and metadata verified against the supplied NovaLCT 0/90/180/270 fixtures.
public sealed class ModuleRotationService(IPointTableParser parser, IScanCodeService scan,
    IModuleValidationService validation) : IModuleRotationService
{
    // Absolute orientation is recognizable only for the canonical running-line
    // layout. Arbitrary wiring tables have no universal absolute-zero marker.
    public int? DetectOrientation(ModuleEditorState state)
    {
        string type = state.Parameters["ScanType"];
        if (!type.StartsWith("Scan_", StringComparison.Ordinal) ||
            !int.TryParse(type.AsSpan(5), out int length) || length is <= 1 or > 128) return null;
        var module = validation.Validate(state);
        var g = module.Geometry;
        var codes = scan.Parse(state.NewScanABCDCode, "NewScanABCDCode");
        var groups = scan.Parse(state.Parameters["DataGroupSequence"], "DataGroupSequence");
        if (codes.Length < length || groups.Length < g.DataGroups || !module.Points.Any(p => !p.IsMissing)) return null;
        bool ascending = codes.Take(length).SequenceEqual(Enumerable.Range(0, length));
        bool descending = codes.Take(length).SequenceEqual(Enumerable.Range(0, length).Reverse());
        bool groupsForward = groups.Take(g.DataGroups).SequenceEqual(Enumerable.Range(0, g.DataGroups));
        bool groupsBackward = groups.Take(g.DataGroups).SequenceEqual(Enumerable.Range(0, g.DataGroups).Reverse());
        int? angle = (g.IsVertical, ascending && groupsForward, descending && groupsBackward) switch
        {
            (false, true, _) => 0, (false, _, true) => 180,
            (true, true, _) => 270, (true, _, true) => 90,
            _ => null
        };
        if (angle is null || module.Points.Count != (g.IsVertical ? g.GroupHeight : g.GroupWidth)) return null;
        for (int i = 0; i < module.Points.Count; i++)
        {
            var point = module.Points[i];
            var expected = angle switch
            {
                0 => new PointTableEntry(i, 0),
                90 => new PointTableEntry(g.GroupWidth - 1, i),
                180 => new PointTableEntry(g.GroupWidth - 1 - i, g.GroupHeight - 1),
                _ => new PointTableEntry(0, g.GroupHeight - 1 - i)
            };
            if (!point.IsMissing && point != expected) return null;
        }
        return angle;
    }
    public void Rotate(ModuleEditorState candidate, Rotation rotation)
    {
        var module = validation.Validate(candidate);
        var g = module.Geometry;
        int scanLength = ScanLength(candidate.Parameters["ScanType"]);
        var sequence = scan.Parse(candidate.Parameters["DataGroupSequence"], "DataGroupSequence");
        if (sequence.Length < g.DataGroups || !sequence.Take(g.DataGroups).Order().SequenceEqual(Enumerable.Range(0, g.DataGroups)))
            throw Messages.Format("GroupSequence", "DataGroupSequence: активные группы должны быть перестановкой 0..DataGroup-1.");
        var rowsByGroup = scan.Parse(candidate.Parameters["RowsCtrlByDataGroup"], "RowsCtrlByDataGroup");
        if (rowsByGroup.Length > 0 && (rowsByGroup.Length < g.DataGroups ||
            rowsByGroup.Take(g.DataGroups).Any(n => n != (g.IsVertical ? g.GroupWidth : g.GroupHeight))))
            throw Messages.Format("UnequalGroups", "Поворот модулей с неодинаковыми размерами групп пока не поддерживается.");
        // The active scan/group order flips when the physical stacking axis reverses.
        bool reverse = rotation == Rotation.HalfTurn ||
            (rotation == Rotation.Right90 && !g.IsVertical) || (rotation == Rotation.Left90 && g.IsVertical);
        if (reverse)
        {
            candidate.Parameters["DataGroupSequence"] = scan.ReversePrefix(candidate.Parameters["DataGroupSequence"], g.DataGroups, "DataGroupSequence");
            var legacy = scan.Parse(candidate.ScanABCDCode, "ScanABCDCode");
            candidate.ScanABCDCode = scan.ReversePrefix(candidate.ScanABCDCode, Math.Min(scanLength, legacy.Length), "ScanABCDCode");
            if (candidate.NewScanABCDCode.Length > 0)
                candidate.NewScanABCDCode = scan.ReversePrefix(candidate.NewScanABCDCode, scanLength, "NewScanABCDCode");
        }
        candidate.PointTableData = parser.Format(module.Points.Select(p => p.IsMissing ? p : rotation switch
        {
            Rotation.Right90 => new PointTableEntry(g.GroupHeight - 1 - p.Y, p.X),
            Rotation.Left90 => new PointTableEntry(p.Y, g.GroupWidth - 1 - p.X),
            Rotation.HalfTurn => new PointTableEntry(g.GroupWidth - 1 - p.X, g.GroupHeight - 1 - p.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(rotation))
        }));
        if (rotation != Rotation.HalfTurn)
        {
            candidate.Parameters["PixelCols"] = g.Rows.ToString(System.Globalization.CultureInfo.InvariantCulture);
            candidate.Parameters["PixelRows"] = g.Cols.ToString(System.Globalization.CultureInfo.InvariantCulture);
            candidate.Parameters["DataDirectType"] = g.IsVertical ? "Horizontal" : "Vertical";
        }
        // LineBias is independent of Max(NewScanABCDCode); all four fixtures retain it.
        validation.Validate(candidate);
        int delta = rotation switch { Rotation.Right90 => 90, Rotation.Left90 => 270, _ => 180 };
        candidate.RotationDegrees = candidate.RotationDegrees is int angle ? (angle + delta) % 360 : null;
        candidate.RelativeRotationDegrees = (candidate.RelativeRotationDegrees + delta) % 360;
    }
    private static int ScanLength(string value) => value.StartsWith("Scan_", StringComparison.Ordinal) &&
        int.TryParse(value.AsSpan(5), out int length) && length is > 0 and <= 128 ? length :
        throw Messages.Format("ScanUnknown", "ScanType: неизвестный формат числа адресов; поворот отменён.");
}
