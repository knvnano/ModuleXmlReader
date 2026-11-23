using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Localization;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Presentation.Services;
using ModuleXmlReader.Presentation.ViewModels;
namespace ModuleXmlReader.Avalonia.Services;

public sealed class AvaloniaDialogService(Window owner, IExternalLinkService links, TextCatalog? catalog = null, IAppLogger? logger = null)
    : IFileDialogService, INotificationService, IAboutDialogService, IUnsavedChangesService
{
    private readonly TextCatalog _text = catalog ?? new TextCatalog();
    private FilePickerFileType ModuleType => new(_text["ModuleFiles"])
        { Patterns = new[] { "*.module" } };

    public async Task<string?> PickOpenFileAsync()
    {
        var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            { Title = _text["OpenTitle"], AllowMultiple = false, FileTypeFilter = new[] { ModuleType } });
        return files.Count == 0 ? null : files[0].Path.LocalPath;
    }
    public async Task<string?> PickSaveFileAsync()
    {
        var file = await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = _text["SaveTitle"], SuggestedFileName = "module_file.module",
            DefaultExtension = "module", FileTypeChoices = new[] { ModuleType }, ShowOverwritePrompt = true
        });
        return file?.Path.LocalPath;
    }
    public Task ShowAsync(string text, string title) => MessageDialog.Show(owner, new Notice(text, title), _text);
    public Task ShowAsync(Notice notice) => MessageDialog.Show(owner, notice, _text);
    public Task ShowAboutAsync()
    {
        var about = new AboutWindow();
        about.DataContext = new AboutViewModel(links, new AvaloniaDialogService(about, links, _text, logger), new WindowCloseService(about), _text, logger);
        about.Closed += (_, _) => ((AboutViewModel)about.DataContext!).Dispose();
        return about.ShowDialog(owner);
    }
    public async Task<UnsavedChangesChoice> ConfirmUnsavedChangesAsync()
    {
        var window = new Window
        {
            Title = _text["UnsavedTitle"], Width = 520, SizeToContent = SizeToContent.Height,
            CanResize = false, WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var buttons = new StackPanel { Orientation = global::Avalonia.Layout.Orientation.Horizontal, Spacing = 10 };
        foreach (var (text, choice) in new[] { ("Save", UnsavedChangesChoice.Save),
            ("Discard", UnsavedChangesChoice.Discard), ("Cancel", UnsavedChangesChoice.Cancel) })
        {
            var button = new Button { Content = _text[text], Tag = text, IsCancel = choice == UnsavedChangesChoice.Cancel };
            button.Click += (_, _) => window.Close(choice);
            buttons.Children.Add(button);
        }
        var prompt = new TextBlock { Text = _text["UnsavedPrompt"] };
        window.Content = new StackPanel
        {
            Margin = new global::Avalonia.Thickness(20), Spacing = 18,
            Children = { prompt, buttons }
        };
        void Update(object? sender, EventArgs e)
        {
            window.Title = _text["UnsavedTitle"]; prompt.Text = _text["UnsavedPrompt"];
            foreach (var button in buttons.Children.OfType<Button>()) button.Content = _text[(string)button.Tag!];
        }
        _text.LanguageChanged += Update;
        try { return await window.ShowDialog<UnsavedChangesChoice>(owner); }
        finally { _text.LanguageChanged -= Update; }
    }
    private sealed class WindowCloseService(Window window) : IWindowCloseService
    {
        public void Close() => window.Close();
    }
}
