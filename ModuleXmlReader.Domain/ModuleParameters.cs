namespace ModuleXmlReader.Domain;

// Raw XML values; semantic validation is performed by Application services.
public sealed class ModuleParameters
{
    public static IReadOnlyList<string> Names { get; } = Array.AsReadOnly(new[]
    {
        "Name",
        "PixelCols",
        "PixelRows",
        "DataDirectType",
        "DriverChipType",
        "DriverChipTypeExtend",
        "DriverTypePro",
        "DecType",
        "OEPolarity",
        "ScanType",
        "TotalPointInTable",
        "DataGroup",
        "DataGroupSequence",
        "ScanABCDCode",
        "RGBCode",
        "RowsCtrlByDataGroup",
        "LineBias",
        "ScreenDriveType",
        "SerialColorNum",
        "SerialDotsNumPerColor",
        "SerialRGBCode",
        "ChipMinLawRepeatNumber",
        "ChannelEnableData",
        "ChannelData",
        "ChipNumber",
        "StartPositionOfDataGroup",
        "ModuleVersion",
        "IsIrregular"
    });
    private readonly Dictionary<string, string> _values = new()
    {
        ["Name"] = string.Empty,
        ["PixelCols"] = string.Empty,
        ["PixelRows"] = string.Empty,
        ["DataDirectType"] = string.Empty,
        ["DriverChipType"] = string.Empty,
        ["DriverChipTypeExtend"] = string.Empty,
        ["DriverTypePro"] = string.Empty,
        ["DecType"] = string.Empty,
        ["OEPolarity"] = string.Empty,
        ["ScanType"] = string.Empty,
        ["TotalPointInTable"] = string.Empty,
        ["DataGroup"] = string.Empty,
        ["DataGroupSequence"] = string.Empty,
        ["ScanABCDCode"] = string.Empty,
        ["RGBCode"] = string.Empty,
        ["RowsCtrlByDataGroup"] = string.Empty,
        ["LineBias"] = string.Empty,
        ["ScreenDriveType"] = string.Empty,
        ["SerialColorNum"] = string.Empty,
        ["SerialDotsNumPerColor"] = string.Empty,
        ["SerialRGBCode"] = string.Empty,
        ["ChipMinLawRepeatNumber"] = string.Empty,
        ["ChannelEnableData"] = string.Empty,
        ["ChannelData"] = string.Empty,
        ["ChipNumber"] = string.Empty,
        ["StartPositionOfDataGroup"] = string.Empty,
        ["ModuleVersion"] = string.Empty,
        ["IsIrregular"] = string.Empty
    };
    [System.Diagnostics.CodeAnalysis.AllowNull]
    public string this[string name]
    {
        get => _values[name];
        set => _values[name] = value ?? string.Empty;
    }
}
