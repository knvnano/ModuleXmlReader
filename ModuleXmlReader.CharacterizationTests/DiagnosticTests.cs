using ModuleXmlReader.Application.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class DiagnosticTests
{
 [Fact] public void ReportCollectsIndependentProblemsRatherThanStoppingAtFirst()
 {
  var e=TestServices.CreateEditor();Assert.True(e.Open(TestServices.Fixture()));
  e.State.PointTableData="0-0-0-0-255-1-999-0-255-255-1";e.State.ScanABCDCode="bad";
  var d=new ModuleDiagnosticService().Analyze(e.State);
  foreach(var key in new[]{"DiagOdd","DiagDuplicate","DiagHalfMissing","DiagByte","DiagMissing","PointCount"})Assert.Contains(d,x=>x.Message.Key==key);
  Assert.Contains(d,x=>x.Field=="ScanABCDCode"&&x.Severity=="SeverityError");
 }
}
