using ModuleXmlReader.Application.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class ScanEditorTests
{
 [Fact] public void ActivePermutationKeepsZeroAndTailAndRejectsCrossingBoundary()
 {
  var s=new ScanCodeService();var result=s.MoveActive("1-0-2-0-0",1,-1,3,"Scan");Assert.Equal("0-1-2-0-0",result);
  Assert.Throws<ArgumentOutOfRangeException>(()=>s.MoveActive(result,2,1,3,"Scan"));
 }
}
