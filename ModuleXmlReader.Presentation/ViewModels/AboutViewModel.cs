using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Services;
namespace ModuleXmlReader.Presentation.ViewModels;

public sealed class AboutViewModel : ObservableObject, IDisposable
{
    public TextCatalog Text { get; }
    public string AboutText => Text["AboutText"];
    public AsyncCommand TelegramCommand { get; }
    public RelayCommand CloseCommand { get; }

    public AboutViewModel(IExternalLinkService links, INotificationService notifications, IWindowCloseService window, TextCatalog? text = null, IAppLogger? logger = null)
    {
        Text = text ?? new TextCatalog(); Text.LanguageChanged += LanguageChanged;
        CloseCommand = new(window.Close);
        TelegramCommand = new(async () =>
        {
            try { links.Open("https://t.me/digitalbudda"); }
            catch (Exception ex) { logger?.Error("External link failed", ex); await notifications.ShowAsync(new Notice("Не удалось открыть ссылку: " + ex.Message, "Ошибка", new("LinkFailed", ex.Message, Messages.FromException(ex)))); }
        });
    }
    private void LanguageChanged(object? sender, EventArgs e) => RaisePropertyChanged(nameof(AboutText));
    public void Dispose() => Text.LanguageChanged -= LanguageChanged;
}
