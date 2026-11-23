using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Presentation.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class DataGroupTests
{
 [Fact] public void GroupChoiceUsesInputOrderAndSurvivesTranslationWithoutMutatingFile()
 {
  var e=TestServices.CreateEditor();Assert.True(e.Open(TestServices.Fixture(90)));var text=new TextCatalog();using var vm=new SimulationViewModel(text,new ManualAnimationClock());vm.UpdateModel(e.State.Preview!.Simulation);
  Assert.Equal(3,vm.GroupChoices.Count);vm.SelectedGroupChoice=vm.GroupChoices[1];int id=vm.SelectedGroup;Assert.Single(vm.Groups);
  text.Toggle();Assert.Equal(id,vm.SelectedGroup);Assert.Single(vm.Groups);Assert.False(e.State.HasChanges);
 }
}
