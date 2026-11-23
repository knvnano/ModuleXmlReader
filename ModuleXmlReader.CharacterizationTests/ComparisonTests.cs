using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Infrastructure;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class ComparisonTests
{
 [Fact] public void FileComparisonReportsCoordinatesCodesAndParametersWithoutMutatingDocument()
 {
  var e=TestServices.CreateEditor();Assert.True(e.Open(TestServices.Fixture(90)));var raw=e.State.PointTableData;
  var service=new ModuleComparisonService(new XmlModuleRepository(new XmlModuleSerializer()));
  Assert.Empty(service.CompareFile(e.State,TestServices.Fixture(90)));
  var diff=service.CompareFile(e.State,TestServices.Fixture());
  Assert.Contains(diff,d=>d.Category=="DiffPoint"&&d.PointNumber==1);Assert.Contains(diff,d=>d.Category=="DiffCode");Assert.Contains(diff,d=>d.Category=="DiffParameter");
  Assert.Equal(raw,e.State.PointTableData);Assert.False(e.State.HasChanges);
 }
}
