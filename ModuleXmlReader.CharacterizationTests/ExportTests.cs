using System.Xml.Linq;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Infrastructure;
using ModuleXmlReader.Presentation.Localization;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class ExportTests : TempTest
{
 [Fact] public void ExportProducesValidSvgAndCompleteReportsWithoutChangingModule()
 {
  var e=TestServices.CreateEditor();Assert.True(e.Open(TestServices.Fixture()));var text=new TextCatalog();var bundle=new ModuleExportService().Build(e.State,k=>text[k],m=>text.Format(m));
  var svg=XDocument.Parse(bundle.Svg);Assert.Equal("svg",svg.Root!.Name.LocalName);Assert.Contains("128",bundle.PointsCsv);Assert.Contains("PixelCols: 128",bundle.Report);
  var writer=new ModuleExportWriter();var folder=writer.Write(Dir,bundle);Assert.True(File.Exists(Path.Combine(folder,"trace.svg")));Assert.True(File.Exists(Path.Combine(folder,"report.md")));Assert.NotEqual(folder,writer.Write(Dir,bundle));Assert.False(e.State.HasChanges);
 }
}
