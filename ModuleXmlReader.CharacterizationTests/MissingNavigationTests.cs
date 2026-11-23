using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Domain;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class MissingNavigationTests
{
 [Fact] public void MissingNavigationWrapsAndHighlightsKnownNeighbours()
 {
  var vm=new PointNavigationViewModel(new TextCatalog());
  vm.Update(new(4,2,[],GapMarkers:new[]{new PreviewGapMarker(0,0,GapEdge.Right,2,2),new PreviewGapMarker(3,0,GapEdge.Left,2,2)}),"0-0-255-255-255-255-3-0");
  vm.NextMissingCommand.Execute(null); Assert.Equal(2,vm.SelectedNumber);Assert.Equal(2,vm.Locations.Count);
  vm.NextMissingCommand.Execute(null); Assert.Equal(3,vm.SelectedNumber);
  vm.NextMissingCommand.Execute(null); Assert.Equal(2,vm.SelectedNumber);
  vm.PreviousMissingCommand.Execute(null);Assert.Equal(3,vm.SelectedNumber);
 }
}
