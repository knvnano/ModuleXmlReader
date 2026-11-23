using System.Xml.Linq;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Application.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class RotationTests : TempTest
{
    public static IEnumerable<object[]> Cases => from angle in new[] { 0, 90, 180, 270 }
        from delta in new[] { 90, 180, 270 } select new object[] { angle, delta };
    [Theory, MemberData(nameof(Cases))]
    public void RotatedSavedDocumentMatchesEveryFieldOfNovaLctFixture(int angle, int delta)
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture(angle)));
        editor.Rotate(delta switch { 90 => Rotation.Right90, 180 => Rotation.HalfTurn, _ => Rotation.Left90 });
        Assert.Empty(editor.State.Notices);
        var output = Path.Combine(Dir, "rotated.module"); Assert.True(editor.Save(output));
        var actual = XDocument.Load(output); var expected = XDocument.Load(TestServices.Fixture((angle + delta) % 360));
        expected.Root!.Element("Module")!.Element("Name")!.Value = actual.Root!.Element("Module")!.Element("Name")!.Value;
        Assert.True(XNode.DeepEquals(expected, actual), $"XML mismatch from {angle} by {delta} degrees");
        Assert.Equal("0", editor.State.Parameters["LineBias"]);
    }
    [Theory]
    [InlineData(0)] [InlineData(90)] [InlineData(180)] [InlineData(270)]
    public void FourRightRotationsAndInverseRotationsRestoreState(int angle)
    {
        var editor = TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture(angle)));
        var points = editor.State.PointTableData; var scan = editor.State.NewScanABCDCode;
        for (int i = 0; i < 4; i++) editor.Rotate(Rotation.Right90);
        Assert.Equal(points, editor.State.PointTableData); Assert.Equal(scan, editor.State.NewScanABCDCode);
        Assert.False(editor.State.HasChanges);
        editor.Rotate(Rotation.Left90); editor.Rotate(Rotation.Right90);
        Assert.Equal(points, editor.State.PointTableData); Assert.False(editor.State.HasChanges);
        editor.Rotate(Rotation.HalfTurn); editor.Rotate(Rotation.HalfTurn);
        Assert.Equal(points, editor.State.PointTableData); Assert.False(editor.State.HasChanges);
    }
    [Fact]
    public void RotationUsesUnsavedEditsAndPreservesMissingPairsAndCodeZero()
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        var parser = new PointTableParser(); var points = parser.Parse(e.State.PointTableData).ToArray();
        points[0] = new(2, 1); points[4] = new(255, 255);
        e.UpdateTables(parser.Format(points), e.State.NewScanABCDCode, e.State.ScanABCDCode);
        Assert.Equal("5", e.State.MissingPixels);
        e.Rotate(Rotation.Right90);
        var rotated = parser.Parse(e.State.PointTableData);
        Assert.Equal(new PointTableEntry(30, 2), rotated[0]); Assert.True(rotated[4].IsMissing);
        Assert.Equal(128, rotated.Count); Assert.Equal("5", e.State.MissingPixels);
        Assert.Equal(254, e.State.Preview!.Points.Count);
        Assert.Equal("0", e.State.NewScanABCDCode.Split('-')[31]);
        Assert.Equal(128, e.State.NewScanABCDCode.Split('-').Length);
    }
    [Theory]
    [InlineData(0, 0, 0)] [InlineData(90, 31, 0)] [InlineData(180, 127, 31)] [InlineData(270, 0, 127)]
    public void PreviewUsesCoordinatePairsAndAllDataGroups(int angle, int firstX, int firstY)
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture(angle)));
        Assert.Equal(new PreviewPoint(firstX, firstY), e.State.Preview!.Points[0]);
        Assert.Equal(256, e.State.Preview.Points.Count);
        Assert.All(e.State.Preview.Points, p => { Assert.InRange(p.X, 0, e.State.Preview.Cols - 1); Assert.InRange(p.Y, 0, e.State.Preview.Rows - 1); });
    }
    [Fact]
    public void ScanReversePreservesZeroPositionsAndPadding()
    {
        var service = new ScanCodeService();
        Assert.Equal("2-0-1-0-9-0", service.ReversePrefix("0-1-0-2-9-0", 4, "test"));
        Assert.Throws<FormatException>(() => service.ReversePrefix("1-2", 3, "test"));
    }
    [Fact]
    public void UnsupportedRotationFailsWithoutPartialChanges()
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        e.State.Parameters["ScanType"] = "Unknown";
        var points = e.State.PointTableData; var preview = e.State.Preview;
        e.Rotate(Rotation.Right90);
        Assert.Equal(points, e.State.PointTableData); Assert.Equal("128", e.State.Parameters["PixelCols"]);
        Assert.Same(preview, e.State.Preview); Assert.Contains("ScanType", Assert.Single(e.State.Notices).Text);
    }
}
