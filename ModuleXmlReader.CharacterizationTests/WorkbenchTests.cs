using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Infrastructure;
using ModuleXmlReader.Application.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;
public sealed class WorkbenchTests
{
 [AvaloniaTheory]
 [InlineData(1120,"ru")]
 [InlineData(1480,"en")]
 public async Task ToolsAndCoordinateEditorsFitAndExportCommandsAreWired(int width,string language)
 {
  var e=TestServices.CreateEditor();var d=new FakeDialogs{OpenPath=TestServices.Fixture()};var text=new TextCatalog();text.SetLanguage(language);
  using var vm=new MainWindowViewModel(e,d,d,d,d,text,comparison:new ModuleComparisonService(new XmlModuleRepository(new XmlModuleSerializer())),exportWriter:new ModuleExportWriter());
  var window=new MainWindow{Width=width,Height=920,DataContext=vm};window.Show();await vm.OpenCommand.ExecuteAsync();Dispatcher.UIThread.RunJobs();window.UpdateLayout();
  var list=window.FindControl<ListBox>("CoordinateList")!;Assert.True(list.Bounds.Height>90,"List height="+list.Bounds.Height);Assert.True(vm.ExportCommand.CanExecute(null));
  vm.Navigation.Number="17";vm.Navigation.GoCommand.Execute(null);Assert.Equal(17,vm.SelectedCoordinate!.Number);
  vm.ShowToolsCommand.Execute(null);Dispatcher.UIThread.RunJobs();window.UpdateLayout();Assert.True(window.FindControl<Grid>("ToolsPage")!.IsVisible);Assert.False(window.FindControl<Grid>("EditorPage")!.IsVisible);
  vm.ToggleLanguageCommand.Execute(null);Assert.True(vm.IsToolsPage);
  vm.ShowEditorCommand.Execute(null);Dispatcher.UIThread.RunJobs();window.UpdateLayout();
  var save=window.FindControl<Button>("SaveButton")!;var point=save.TranslatePoint(default,window);Assert.NotNull(point);Assert.True(point.Value.X+save.Bounds.Width<=window.ClientSize.Width+1);
  var path=Environment.GetEnvironmentVariable("MODULE_READER_WORKBENCH_SCREENSHOT");
  if(path is not null&&width==1480){e.State.OpenedFilePath="Samples/128_64_normal.module";text.SetLanguage("en");Dispatcher.UIThread.RunJobs();window.UpdateLayout();using var frame=window.CaptureRenderedFrame();Assert.NotNull(frame);frame.Save(path,PngBitmapEncoderOptions.Default);}
  window.Close();
 }
}
