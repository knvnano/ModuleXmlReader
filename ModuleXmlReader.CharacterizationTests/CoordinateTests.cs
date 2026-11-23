using Avalonia.Controls;
using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Localization;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public class CoordinateTests
{
 [global::Avalonia.Headless.XUnit.AvaloniaFact] public async Task TableAndCanvasSelectionStayInSyncAndApplySupportsUndo()
 {
  var e=TestServices.CreateEditor();var dialogs=new FakeDialogs {OpenPath=TestServices.Fixture()};using var vm=new MainWindowViewModel(e,dialogs,dialogs,dialogs,dialogs);
  var w=new ModuleXmlReader.Avalonia.MainWindow {DataContext=vm};w.Show();await vm.OpenCommand.ExecuteAsync();global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
  vm.SelectedCoordinate=vm.Coordinates[16];Assert.Equal(17,vm.Navigation.SelectedNumber);Assert.Equal(2,w.FindControl<ModuleXmlReader.Avalonia.ModuleViewport>("ModuleCanvas")!.Preview.Selection.Count);
  vm.Navigation.Select(20);Assert.Equal(20,vm.SelectedCoordinate!.Number);
  vm.SelectedCoordinate.X="255";vm.SelectedCoordinate.Y="255";vm.SelectedCoordinate.ApplyCommand.Execute(null);Assert.True(e.CanUndo);Assert.Contains(20,vm.Navigation.Missing.Select(p=>p.Number));
  await vm.UndoCommand.ExecuteAsync();Assert.False(vm.HasChanges);Assert.DoesNotContain(20,vm.Navigation.Missing.Select(p=>p.Number));w.Close();
 }
 [Fact] public void CoordinateEditorRejectsHalfMissingAndAppliesValidPairOnce()
 {
  int calls=0;var row=new CoordinateRowViewModel(2,1,0,new TextCatalog(),(n,x,y)=>{calls++;return true;});
  row.X="255";row.ApplyCommand.Execute(null);Assert.Equal(0,calls);
  row.Y="255";row.ApplyCommand.Execute(null);Assert.Equal(1,calls);
  row.X="-1";row.ApplyCommand.Execute(null);Assert.Equal(1,calls);
 }
}
