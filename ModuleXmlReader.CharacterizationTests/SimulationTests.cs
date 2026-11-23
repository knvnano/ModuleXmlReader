using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ModuleXmlReader.Avalonia;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Presentation.Services;
using ModuleXmlReader.Presentation.ViewModels;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class SimulationTests
{
    private static SimulationModel Model() => new(new(4,4,2,false),
        new PointTableEntry[] { new(0,0), new(255,255), new(2,0), new(3,0) },
        new[] {0,1}, new SimulationGroup[] {new(1,0,0,0,4,2),new(0,1,0,2,4,2)}, true);
    [Fact]
    public void MissingSlotsShiftAndGroupsLatchInParallelWithOutputBlanking()
    {
        var e = new ModuleSimulationEngine(Model());
        Assert.Empty(e.Frame().Groups[0].LitPoints); Assert.True(e.Frame().OeLevel);
        e.Step(); Assert.True(e.Frame().ClockHigh); Assert.Equal(1,e.Frame().Groups[0].Register[0].Number);
        e.Step(); e.Step(); Assert.True(e.Frame().Groups[0].Register[0].IsMissing);
        for(int i=3;i<8;i++) e.Step();
        Assert.Equal(SimulationPhase.Latch,e.Phase); Assert.True(e.Frame().LatchHigh);
        Assert.False(e.Frame().OutputsEnabled);
        e.Step(); var frame=e.Frame(); Assert.False(frame.OeLevel);
        Assert.All(frame.Groups,g=>Assert.Equal(3,g.LitPoints.Count));
        Assert.Equal(new PreviewPoint(0,2),frame.Groups[1].LitPoints[0]);
        e.Step(); Assert.Equal(0,e.Frame().DisplayAddress); Assert.True(e.Frame().OutputsEnabled);
        for(int i=0;i<8;i++) e.Step(); Assert.False(e.Frame().OutputsEnabled);
        e.Step(); Assert.Equal(1,e.Frame().DisplayAddress);
        Assert.Equal(new PreviewPoint(0,3),e.Frame().Groups[1].LitPoints[0]);
        e.Reset(); Assert.Null(e.Frame().DisplayAddress); Assert.All(e.Frame().Groups[0].Register,p=>Assert.Equal(0,p.Number));
    }
    [Theory]
    [InlineData(0)] [InlineData(90)] [InlineData(180)] [InlineData(270)]
    public void RealRotatedModulesKeepAllScanPointsInsideTheirGroups(int angle)
    {
        var editor=TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture(angle)));
        var model=editor.State.Preview!.Simulation!; Assert.Null(model.Problem); Assert.Equal(2,model.Groups.Count);
        var e=new ModuleSimulationEngine(model);
        for(int row=0;row<32;row++)
        {
            while(e.Phase!=SimulationPhase.Output)e.Step();
            foreach(var group in e.Frame().Groups)
                Assert.All(group.LitPoints,p=> { Assert.InRange(p.X,group.Group.X,group.Group.X+group.Group.Width-1); Assert.InRange(p.Y,group.Group.Y,group.Group.Y+group.Group.Height-1); });
            e.Step();
        }
        Assert.False(editor.State.HasChanges);
    }
    [Fact]
    public void ClockPauseAndModelReplacementAreDeterministic()
    {
        var clock=new ManualAnimationClock(); using var vm=new SimulationViewModel(new TextCatalog(),clock);
        var model=Model(); vm.UpdateModel(model); vm.PlayPauseCommand.Execute(null); clock.Advance();
        Assert.NotNull(vm.Frame); var frame=vm.Frame;
        vm.Pause(); clock.Advance(); Assert.Same(frame,vm.Frame);
        vm.UpdateModel(model); Assert.Same(frame,vm.Frame);
        vm.StepCommand.Execute(null); Assert.NotSame(frame,vm.Frame);
        vm.ResetCommand.Execute(null); Assert.Null(vm.Frame);
        vm.UpdateModel(model with { Problem=new("SimNoAddresses","No scan addresses") }); Assert.False(vm.CanSimulate);
    }
    [AvaloniaFact]
    public void PanelBindsAndCanvasRendersConcurrentLedRows()
    {
        var editor=TestServices.CreateEditor(); Assert.True(editor.Open(TestServices.Fixture()));
        using var vm=new SimulationViewModel(new TextCatalog(),new ManualAnimationClock()); vm.UpdateModel(editor.State.Preview!.Simulation);
        for(int i=0;i<257;i++) vm.StepCommand.Execute(null);
        var canvas=new ModuleViewport { Data=editor.State.Preview, Simulation=vm };
        var window=new Window {Width=1200,Height=650,Content=canvas}; window.Show(); Dispatcher.UIThread.RunJobs(); window.UpdateLayout(); canvas.FitToView();
        Assert.Same(vm.Frame,canvas.Preview.Frame); Assert.True(canvas.Preview.ShowGroups);
        using var bitmap=window.CaptureRenderedFrame(); Assert.NotNull(bitmap);
        var path=Environment.GetEnvironmentVariable("MODULE_READER_SIM_SCREENSHOT"); if(path is not null)bitmap.Save(path,PngBitmapEncoderOptions.Default);
        window.Close();
    }
}
