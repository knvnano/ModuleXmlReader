using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Localization;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public sealed class PointNavigationTests
{
 [Fact] public void JumpSelectsExactRawPairAndAllGroupLocationsWithoutEditing()
 {
  var editor=TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
  var vm=new PointNavigationViewModel(new TextCatalog()); vm.Update(editor.State.Preview,editor.State.PointTableData); vm.Number="17";vm.Go();
  Assert.Equal("16-0",editor.State.PointTableData[vm.SelectionStart..vm.SelectionEnd]); Assert.Equal(2,vm.Locations.Count); Assert.False(editor.State.HasChanges);
  vm.Number="99999";vm.Go(); Assert.NotEmpty(vm.Error);Assert.Equal(17,vm.SelectedNumber);
 }
}
