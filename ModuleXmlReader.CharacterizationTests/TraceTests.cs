using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Presentation.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class TraceTests
{
 [Fact] public void TraceStepsWithoutChangingScanEngineAndResetsOnFileChange()
 {
  var e=TestServices.CreateEditor();Assert.True(e.Open(TestServices.Fixture()));var clock=new ManualAnimationClock();using var vm=new SimulationViewModel(new TextCatalog(),clock);vm.UpdateModel(e.State.Preview!.Simulation);vm.TraceMode=true;
  vm.StepCommand.Execute(null);Assert.Equal(1,vm.TraceNumber);Assert.Null(vm.Frame);
  vm.StepCommand.Execute(null);Assert.Equal(2,vm.TraceNumber);vm.ResetCommand.Execute(null);Assert.Equal(0,vm.TraceNumber);Assert.False(e.State.HasChanges);
 }
}
