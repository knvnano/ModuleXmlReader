using ModuleXmlReader.Infrastructure;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class BatchTests : TempTest
{
 [Fact] public void BatchKeepsSourcesAndEarlierOutputsAndReportsFailures()
 {
  var input=Path.Combine(Dir,"input");Directory.CreateDirectory(input);var source=Path.Combine(input,"source.module");File.Copy(TestServices.Fixture(),source);File.WriteAllText(Path.Combine(input,"bad.module"),"bad");var original=File.ReadAllBytes(source);
  var batch=new ModuleBatchService(()=>TestServices.CreateEditor());var first=batch.Process(input,Dir,true,TestContext.Current.CancellationToken);Assert.Equal(4,first.Count(r=>r.Success));Assert.Single(first,r=>!r.Success);Assert.Equal(original,File.ReadAllBytes(source));
  foreach(var r in first.Where(r=>r.Success)){var e=TestServices.CreateEditor();Assert.True(e.Open(r.Output));Assert.Equal(r.RelativeAngle,e.State.RotationDegrees);}
  var second=batch.Process(input,Dir,true,TestContext.Current.CancellationToken);Assert.Empty(first.Where(r=>r.Success).Select(r=>r.Output).Intersect(second.Where(r=>r.Success).Select(r=>r.Output)));
  var cancelled=batch.Process(input,null,false,new CancellationToken(true));Assert.All(cancelled,r=>Assert.Equal("BatchCancelled",r.Message.Key));
 }
}
