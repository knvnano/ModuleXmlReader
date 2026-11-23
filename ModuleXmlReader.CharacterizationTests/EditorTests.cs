using System.Xml.Linq;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Infrastructure;
using ModuleXmlReader.Domain;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class EditorTests : TempTest
{
    [Theory]
    [InlineData("bad-xml")] [InlineData("no-module")] [InlineData("points")]
    [InlineData("dimensions")] [InlineData("group")] [InlineData("code")]
    public void FailedOpenPreservesCurrentDocumentEditsPathAndPreview(string problem)
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        var edited = "1-1-" + string.Join('-', e.State.PointTableData.Split('-').Skip(2));
        e.UpdateTables(edited, e.State.NewScanABCDCode, e.State.ScanABCDCode);
        var preview = e.State.Preview;
        var invalid = XDocument.Load(TestServices.Fixture()); var m = invalid.Root!.Element("Module")!;
        switch (problem)
        {
            case "no-module": m.Remove(); break;
            case "points": m.Element("PointTableData")!.Value = "1-2-3"; break;
            case "dimensions": m.Element("PixelCols")!.Value = "0"; break;
            case "group": m.Element("DataGroup")!.Value = "3"; break;
            case "code": m.Element("ScanABCDCode")!.Value = "abc"; break;
        }
        var path = Path.Combine(Dir, "bad.module"); invalid.Save(path);
        if (problem == "bad-xml") File.WriteAllText(path, "<Config>");
        Assert.False(e.Open(path)); Assert.Equal(TestServices.Fixture(), e.State.OpenedFilePath);
        Assert.Equal(edited, e.State.PointTableData); Assert.Same(preview, e.State.Preview); Assert.True(e.State.HasChanges);
        var saved = Path.Combine(Dir, "saved.module"); Assert.True(e.Save(saved));
        Assert.Equal(edited, XDocument.Load(saved).Root!.Element("Module")!.Element("PointTableData")!.Value);
    }
    [Theory]
    [InlineData("1-2-3")] [InlineData("abc-0")] [InlineData("256-0")]
    [InlineData("255-0")] [InlineData("0-255")] [InlineData("-1-0")] [InlineData("")]
    public void InvalidEditsClearPreviewBlockSavingAndKeepDestination(string points)
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        e.UpdateTables(points, e.State.NewScanABCDCode, e.State.ScanABCDCode);
        Assert.NotEmpty(e.State.ValidationError); Assert.Null(e.State.Preview); Assert.True(e.State.HasChanges);
        var destination = Path.Combine(Dir, "existing.module"); File.WriteAllText(destination, "keep me");
        Assert.False(e.Save(destination)); Assert.Equal("keep me", File.ReadAllText(destination));
    }
    [Fact]
    public void CorrectingEditsRestoresPreviewAndDirtyFlag()
    {
        var e = TestServices.CreateEditor(); Assert.True(e.Open(TestServices.Fixture()));
        var valid = e.State.PointTableData;
        e.UpdateTables("invalid", e.State.NewScanABCDCode, e.State.ScanABCDCode);
        e.UpdateTables(valid, e.State.NewScanABCDCode, e.State.ScanABCDCode);
        Assert.Empty(e.State.ValidationError); Assert.NotNull(e.State.Preview); Assert.False(e.State.HasChanges);
    }
    [Fact]
    public void SaveUsesLoadedSnapshotPreservesUnknownXmlAndUpdatesSaveAsPath()
    {
        var source = Path.Combine(Dir, "source.module"); var doc = XDocument.Load(TestServices.Fixture());
        doc.Root!.Add(new XComment("keep"), new XElement("Vendor", new XAttribute("v", "1"), new XElement("Nested", "original")));
        doc.Save(source); var e = TestServices.CreateEditor(); Assert.True(e.Open(source));
        File.WriteAllText(source, "external changes");
        e.Rotate(Rotation.HalfTurn);
        var destination = Path.Combine(Dir, "copy.module"); Assert.True(e.Save(destination));
        var saved = XDocument.Load(destination);
        Assert.Equal("original", saved.Root!.Element("Vendor")!.Element("Nested")!.Value);
        Assert.Contains(saved.Root.Nodes(), n => n is XComment c && c.Value == "keep");
        Assert.Equal(destination, e.State.OpenedFilePath); Assert.False(e.State.HasChanges);
        File.Delete(source); e.Rotate(Rotation.Right90); Assert.True(e.Save(destination));
    }
    [Fact]
    public void MissingEditableElementsAreCreatedWhenSaved()
    {
        var source = Path.Combine(Dir, "source.module"); var doc = XDocument.Load(TestServices.Fixture());
        var m = doc.Root!.Element("Module")!; var scan = m.Element("ScanABCDCode")!.Value; var newScan = m.Element("NewScanABCDCode")!.Value;
        m.Element("ScanABCDCode")!.Remove(); m.Element("NewScanABCDCode")!.Remove(); doc.Save(source);
        var e = TestServices.CreateEditor(); Assert.True(e.Open(source)); e.UpdateTables(e.State.PointTableData, newScan, scan);
        var output = Path.Combine(Dir, "output.module"); Assert.True(e.Save(output));
        m = XDocument.Load(output).Root!.Element("Module")!;
        Assert.Equal(scan, m.Element("ScanABCDCode")!.Value); Assert.Equal(newScan, m.Element("NewScanABCDCode")!.Value);
    }
    [Fact]
    public void SerializationFailurePreservesDestinationAndDirtyStateAndCleansTemporaryFile()
    {
        var repository = new XmlModuleRepository(new FailingSerializer());
        var e = TestServices.CreateEditor(repository); Assert.True(e.Open(TestServices.Fixture())); e.Rotate(Rotation.HalfTurn);
        var destination = Path.Combine(Dir, "target.module"); File.WriteAllText(destination, "original content");
        Assert.False(e.Save(destination)); Assert.Equal("original content", File.ReadAllText(destination));
        Assert.Equal(TestServices.Fixture(), e.State.OpenedFilePath); Assert.True(e.State.HasChanges);
        Assert.Empty(Directory.GetFiles(Dir, "*.tmp"));
    }
    [Fact]
    public void InitialSaveAndRotationFailWithoutCreatingFiles()
    {
        var e = TestServices.CreateEditor(); var path = Path.Combine(Dir, "out.module");
        Assert.False(e.Save(path)); e.Rotate(Rotation.HalfTurn);
        Assert.False(File.Exists(path)); Assert.False(e.State.IsLoaded); Assert.Equal(2, e.State.Notices.Count);
    }
    private sealed class FailingSerializer : IModuleSerializer
    {
        private readonly XmlModuleSerializer _real = new();
        public IModuleDocument Deserialize(string path) => _real.Deserialize(path);
        public void Serialize(IModuleDocument document, string path) { File.WriteAllText(path, "partial"); throw new IOException("simulated write failure"); }
    }
}
