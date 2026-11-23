using ModuleXmlReader.Domain;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Presentation.Services;
using ModuleXmlReader.Presentation.ViewModels;
using Xunit;
namespace ModuleXmlReader.CharacterizationTests;

public sealed class PresentationTests : TempTest
{
    [Fact]
    public async Task EditsUpdatePreviewValidationAndCommandAvailabilityImmediately()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture() };
        var vm = Create(editor, dialogs);
        Assert.False(vm.SaveCommand.CanExecute(null)); Assert.True(vm.IsEditorReadOnly);
        await vm.OpenCommand.ExecuteAsync(); var original = vm.PointTableData; var preview = vm.Preview;
        vm.PointTableData = "1-2-3";
        Assert.True(vm.HasChanges); Assert.Null(vm.Preview); Assert.NotEmpty(vm.ValidationError); Assert.False(vm.SaveCommand.CanExecute(null));
        vm.PointTableData = original;
        Assert.NotNull(vm.Preview); Assert.NotSame(preview, vm.Preview); Assert.Empty(vm.ValidationError); Assert.False(vm.HasChanges);
        Assert.True(vm.RotateCommand.CanExecute(null)); await vm.RotateCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync();
        Assert.StartsWith("127-31", vm.PointTableData); Assert.True(vm.HasChanges);
        Assert.Equal(vm.ScanABCDCode, vm.Parameters.Single(p => p.Name == "ScanABCDCode").Value);
    }
    [Fact]
    public async Task CancelledFileDialogsAndUnsavedPromptKeepCurrentEdits()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture() }; var vm = Create(editor, dialogs);
        await vm.OpenCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync(); var edited = vm.PointTableData;
        dialogs.OpenPath = null; await vm.OpenCommand.ExecuteAsync(); Assert.Equal(0, dialogs.ConfirmCalls);
        dialogs.OpenPath = TestServices.Fixture(90); dialogs.Choice = UnsavedChangesChoice.Cancel;
        await vm.OpenCommand.ExecuteAsync(); Assert.Equal(edited, vm.PointTableData); Assert.True(vm.HasChanges);
        await vm.SaveCommand.ExecuteAsync(); Assert.Equal(edited, vm.PointTableData); Assert.True(vm.HasChanges);
        Assert.False(await vm.RequestCloseAsync());
    }
    [Fact]
    public async Task SaveBeforeOpenAndCloseHonorsFailureCancellationAndSuccess()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture(), Choice = UnsavedChangesChoice.Save };
        var vm = Create(editor, dialogs); await vm.OpenCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync();
        Assert.False(await vm.RequestCloseAsync()); // save dialog cancelled
        dialogs.SavePath = Dir; Assert.False(await vm.RequestCloseAsync()); Assert.True(vm.HasChanges); // write failure
        dialogs.SavePath = Path.Combine(Dir, "saved.module"); Assert.True(await vm.RequestCloseAsync()); Assert.False(vm.HasChanges);
        Assert.Equal(dialogs.SavePath, vm.OpenedFilePath);
        await vm.RotateCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync(); dialogs.OpenPath = TestServices.Fixture(90);
        await vm.OpenCommand.ExecuteAsync(); Assert.Equal(TestServices.Fixture(90), vm.OpenedFilePath); Assert.False(vm.HasChanges);
        Assert.True(File.Exists(dialogs.SavePath));
    }
    [Fact]
    public async Task DiscardAllowsOpenWithoutOverwritingOldFile()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenPath = TestServices.Fixture(), Choice = UnsavedChangesChoice.Discard };
        var vm = Create(editor, dialogs); await vm.OpenCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync(); await vm.RotateCommand.ExecuteAsync();
        dialogs.OpenPath = TestServices.Fixture(90); await vm.OpenCommand.ExecuteAsync();
        Assert.Equal("64", vm.Parameters.Single(p => p.Name == "PixelCols").Value); Assert.False(vm.HasChanges); Assert.Empty(dialogs.Notices);
    }
    [Fact]
    public async Task InProgressOperationDisablesCommandsAndPreventsDuplicateExecution()
    {
        var editor = TestServices.CreateEditor(); var gate = new TaskCompletionSource<string?>();
        var dialogs = new FakeDialogs { PendingOpen = gate.Task }; var vm = Create(editor, dialogs);
        var first = vm.OpenCommand.ExecuteAsync(); Assert.True(vm.IsBusy);
        Assert.False(vm.OpenCommand.CanExecute(null)); Assert.False(vm.SaveCommand.CanExecute(null));
        await vm.OpenCommand.ExecuteAsync(); Assert.Equal(1, dialogs.OpenCalls); Assert.False(await vm.RequestCloseAsync());
        gate.SetResult(TestServices.Fixture()); await first;
        Assert.False(vm.IsBusy); Assert.True(vm.SaveCommand.CanExecute(null));
    }
    [Fact]
    public async Task DialogFailuresAreReportedAndCommandsRecover()
    {
        var editor = TestServices.CreateEditor(); var dialogs = new FakeDialogs { OpenError = new IOException("picker failure") }; var vm = Create(editor, dialogs);
        await vm.OpenCommand.ExecuteAsync(); Assert.Equal(("picker failure", "Ошибка"), Assert.Single(dialogs.Notices));
        Assert.False(vm.IsBusy); Assert.True(vm.OpenCommand.CanExecute(null));
    }
    [Fact]
    public async Task AboutUsesLinkAndCloseServicesAndReportsLinkErrors()
    {
        var dialogs = new FakeDialogs(); var links = new FakeLinks(); var close = new FakeClose();
        var vm = new AboutViewModel(links, dialogs, close);
        await vm.TelegramCommand.ExecuteAsync(); Assert.Equal("https://t.me/digitalbudda", links.Url);
        links.Error = new InvalidOperationException("test error"); await vm.TelegramCommand.ExecuteAsync();
        Assert.Equal(("Не удалось открыть ссылку: test error", "Ошибка"), Assert.Single(dialogs.Notices));
        vm.CloseCommand.Execute(null); Assert.True(close.Closed);
    }
    private static MainWindowViewModel Create(IModuleEditorService editor, FakeDialogs dialogs) => new(editor, dialogs, dialogs, dialogs, dialogs);
}
internal sealed class FakeDialogs : IFileDialogService, INotificationService, IAboutDialogService, IUnsavedChangesService
{
    public string? OpenPath { get; set; }
    public string? SavePath { get; set; }
    public Task<string?>? PendingOpen { get; set; }
    public Exception? OpenError { get; set; }
    public UnsavedChangesChoice Choice { get; set; } = UnsavedChangesChoice.Cancel;
    public int ConfirmCalls { get; private set; }
    public int OpenCalls { get; private set; }
    public List<(string Text, string Title)> Notices { get; } = new();
    public Task<string?> PickOpenFileAsync()
    {
        OpenCalls++; if (OpenError is not null) throw OpenError;
        return PendingOpen ?? Task.FromResult(OpenPath);
    }
    public Task<string?> PickSaveFileAsync() => Task.FromResult(SavePath);
    public Task ShowAsync(string text, string title) { Notices.Add((text, title)); return Task.CompletedTask; }
    public Task ShowAboutAsync() => Task.CompletedTask;
    public Task<UnsavedChangesChoice> ConfirmUnsavedChangesAsync() { ConfirmCalls++; return Task.FromResult(Choice); }
}
internal sealed class FakeLinks : IExternalLinkService
{
    public string? Url { get; private set; }
    public Exception? Error { get; set; }
    public void Open(string url) { Url = url; if (Error is not null) throw Error; }
}
internal sealed class FakeClose : IWindowCloseService
{
    public bool Closed { get; private set; }
    public void Close() => Closed = true;
}
