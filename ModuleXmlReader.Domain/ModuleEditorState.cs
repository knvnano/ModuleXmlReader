namespace ModuleXmlReader.Domain;

public sealed class ModuleEditorState
{
    public ModuleParameters Parameters { get; } = new();
    public string OpenedFilePath { get; set; } = string.Empty;
    public bool IsLoaded => OpenedFilePath.Length > 0;
    public int? RotationDegrees { get; set; }
    public int RelativeRotationDegrees { get; set; }
    public bool HasChanges { get; set; }
    public string PointTableData { get; set; } = string.Empty;
    public string NewScanABCDCode { get; set; } = string.Empty;
    public string ScanABCDCode { get; set; } = string.Empty;
    public string MissingPixels { get; set; } = "Загрузите файл модуля";
    public MessageToken? ValidationProblem { get; set; }
    public string ValidationError { get; set; } = string.Empty;
    public PreviewData? Preview { get; set; }
    public List<Notice> Notices { get; } = new();
}

public sealed record Notice(string Text, string Title, MessageToken? Message = null, string TitleKey = "Error");
public readonly record struct PointTableEntry(int X, int Y)
{
    public bool IsMissing => X == 255 && Y == 255;
}
public readonly record struct PreviewPoint(int X, int Y);
public readonly record struct PreviewScanAddress(int X, int Y, int Index);
public readonly record struct PreviewPixel(int X, int Y, int Number, bool IsMissing = false, bool IsInferred = false);
public enum GapEdge { Left, Right, Top, Bottom }
public readonly record struct PreviewGapMarker(int X, int Y, GapEdge Edge, int FirstMissing, int Count);
public sealed record PreviewData(int Cols, int Rows, IReadOnlyList<PreviewPoint> Points, IReadOnlyList<PreviewScanAddress>? ScanLine = null, IReadOnlyList<PreviewPixel>? Pixels = null, IReadOnlyList<int>? UnlocatedMissing = null, IReadOnlyList<PreviewGapMarker>? GapMarkers = null, SimulationModel? Simulation = null);
public enum Rotation { Left90, Right90, HalfTurn }
public sealed record ModuleGeometry(int Cols, int Rows, int DataGroups, bool IsVertical)
{
    public int GroupWidth => IsVertical ? Cols / DataGroups : Cols;
    public int GroupHeight => IsVertical ? Rows : Rows / DataGroups;
}
public sealed record ValidatedModule(ModuleGeometry Geometry, IReadOnlyList<PointTableEntry> Points, IReadOnlyList<int>? ScanAddresses = null);
