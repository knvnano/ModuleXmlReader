using System.Xml.Linq;
using ModuleXmlReader.Presentation.ViewModels;
using ModuleXmlReader.Presentation.Services;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class RotationButtonTests : TempTest
{
    [Theory]
    [InlineData(0)] [InlineData(90)] [InlineData(180)] [InlineData(270)]
    public async Task ButtonStartsAtLoadedOrientationAndCyclesThroughFourPositions(int initial)
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture(initial) };
        var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs);
        Assert.Equal("Поворот: —", vm.RotationLabel); Assert.False(vm.RotateCommand.CanExecute(null));
        await vm.OpenCommand.ExecuteAsync(); Assert.Equal($"Поворот: {initial}°", vm.RotationLabel);
        var points = vm.PointTableData;
        for (int step = 1; step <= 4; step++)
        {
            await vm.RotateCommand.ExecuteAsync();
            Assert.Equal($"Поворот: {(initial + step * 90) % 360}°", vm.RotationLabel);
        }
        Assert.Equal(points, vm.PointTableData); Assert.False(vm.HasChanges);
    }
    [Fact]
    public async Task FailedRotationAndFailedOpenDoNotAdvanceDisplayedAngle()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture(90) };
        var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs);
        await vm.OpenCommand.ExecuteAsync(); editor.State.Parameters["ScanType"] = "Unknown";
        await vm.RotateCommand.ExecuteAsync(); Assert.Equal("Поворот: 90°", vm.RotationLabel);
        dialogs.OpenPath = Path.Combine(Dir, "missing.module"); await vm.OpenCommand.ExecuteAsync();
        Assert.Equal("Поворот: 90°", vm.RotationLabel);
    }
    [Fact]
    public async Task SaveAndReopenRetainAngleAndAnotherFileSetsItsOwnPosition()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture(), SavePath = Path.Combine(Dir, "saved.module") };
        var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs);
        await vm.OpenCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync(); await vm.SaveCommand.ExecuteAsync();
        Assert.Equal("Поворот: 90°", vm.RotationLabel);
        dialogs.OpenPath = dialogs.SavePath; await vm.OpenCommand.ExecuteAsync(); Assert.Equal("Поворот: 90°", vm.RotationLabel);
        dialogs.OpenPath = TestServices.Fixture(270); await vm.OpenCommand.ExecuteAsync(); Assert.Equal("Поворот: 270°", vm.RotationLabel);
        await vm.RotateCommand.ExecuteAsync(); Assert.Equal("Поворот: 0°", vm.RotationLabel);
    }
    [Fact]
    public async Task ArbitraryWiringReportsRelativeRotationInsteadOfGuessingAbsoluteAngle()
    {
        var doc = XDocument.Load(TestServices.Fixture());
        doc.Root!.Element("Module")!.Element("PointTableData")!.Value = "1-1-" + string.Join('-',
            doc.Root.Element("Module")!.Element("PointTableData")!.Value.Split('-').Skip(2));
        var path = Path.Combine(Dir, "custom.module"); doc.Save(path);
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = path };
        var vm = new MainWindowViewModel(editor, dialogs, dialogs, dialogs, dialogs);
        await vm.OpenCommand.ExecuteAsync(); Assert.Equal("Поворот: 0° от исходного", vm.RotationLabel);
        await vm.RotateCommand.ExecuteAsync(); Assert.Equal("Поворот: 90° от исходного", vm.RotationLabel);
    }
}
