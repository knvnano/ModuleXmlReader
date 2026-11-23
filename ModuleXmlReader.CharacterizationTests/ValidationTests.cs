using ModuleXmlReader.Domain;
using ModuleXmlReader.Application.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public sealed class ValidationTests
{
    [Theory]
    [InlineData(Rotation.Left90, 1, 0)]
    [InlineData(Rotation.Right90, 0, 2)]
    [InlineData(Rotation.HalfTurn, 0, 0)]
    public void CoordinateTransformUsesBothCoordinatesAndActualGroupDimensions(Rotation rotation, int x, int y)
    {
        var state = new ModuleEditorState { PointTableData = "2-1-0-0-1-0-2-0-0-1-255-255",
            ScanABCDCode = "0-1-0-0", NewScanABCDCode = "0-1-0-0" };
        state.Parameters["PixelCols"] = "3"; state.Parameters["PixelRows"] = "4";
        state.Parameters["DataGroup"] = "2"; state.Parameters["DataDirectType"] = "Horizontal";
        state.Parameters["DataGroupSequence"] = "0-1-2-3-4-5-6-7";
        state.Parameters["RowsCtrlByDataGroup"] = "2-2"; state.Parameters["ScanType"] = "Scan_2";
        state.Parameters["TotalPointInTable"] = "6";
        var parser = new PointTableParser(); var scan = new ScanCodeService();
        var validation = new ModuleValidationService(parser, scan); var service = new ModuleRotationService(parser, scan, validation);
        var before = state.PointTableData; service.Rotate(state, rotation);
        Assert.Equal(new PointTableEntry(x, y), parser.Parse(state.PointTableData)[0]);
        Assert.True(parser.Parse(state.PointTableData)[5].IsMissing);
        Assert.Equal(new[] { "0", "0" }, state.ScanABCDCode.Split('-').Skip(2));
        service.Rotate(state, rotation switch { Rotation.Left90 => Rotation.Right90, Rotation.Right90 => Rotation.Left90, _ => Rotation.HalfTurn });
        Assert.Equal(before, state.PointTableData); Assert.Equal("0-1-0-0", state.ScanABCDCode);
    }
    [Theory]
    [InlineData("128-0", "границы")]
    [InlineData("0-32", "границы")]
    public void CoordinateBoundsAreCheckedForWholePairs(string pair, string message)
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        var values = e.State.PointTableData.Split('-');
        e.UpdateTables(pair + "-" + string.Join('-', values.Skip(2)), e.State.NewScanABCDCode, e.State.ScanABCDCode);
        Assert.Contains(message, e.State.ValidationError); Assert.Null(e.State.Preview);
    }
    [Fact]
    public void PointCountAndScanLengthAreValidatedWithoutAssumingFullPixelGrid()
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        Assert.Equal("128", e.State.Parameters["TotalPointInTable"]); // 8192 physical pixels, only 128 table entries
        var original = e.State.PointTableData;
        e.UpdateTables(string.Join('-', original.Split('-').Skip(2)), e.State.NewScanABCDCode, e.State.ScanABCDCode);
        Assert.Contains("TotalPointInTable", e.State.ValidationError);
        e.UpdateTables(original, "0-1", e.State.ScanABCDCode);
        Assert.Contains("NewScanABCDCode", e.State.ValidationError);
    }
}
