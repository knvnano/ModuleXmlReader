using System.Globalization;
using System.Text.Json;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Presentation.Localization;

public sealed class TextCatalog : ObservableObject
{
    private static readonly IReadOnlyDictionary<string, string> Russian = Load("ru"), English = Load("en");
    private string _language = "ru";
    public event EventHandler? LanguageChanged;
    public string Language => _language;
    public string this[string key] => (_language == "ru" ? Russian : English).TryGetValue(key, out var value) ? value : key;
    public IReadOnlyCollection<string> Keys => Russian.Keys.ToArray();
    public bool HasTranslation(string key) => Russian.ContainsKey(key) && English.ContainsKey(key);
    public void Toggle() => SetLanguage(_language == "ru" ? "en" : "ru");
    public void SetLanguage(string language)
    {
        if (language is not ("ru" or "en")) throw new ArgumentOutOfRangeException(nameof(language));
        if (_language == language) return;
        _language = language; RaisePropertyChanged(nameof(Language)); RaisePropertyChanged("Item[]"); RaisePropertyChanged("Item");
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
    public string Format(string key, params object[] args) => string.Format(CultureInfo.InvariantCulture, this[key], args);
    public string Format(MessageToken token) => (_language == "ru" ? Russian : English).TryGetValue(token.Key, out var template)
        ? string.Format(CultureInfo.InvariantCulture, template, token.Arguments.Select(a => a is MessageToken nested ? Format(nested) : a).ToArray())
        : token.Fallback;
    public string NoticeText(Notice notice) => notice.Message is { } message ? Format(message) : notice.Text;
    private static IReadOnlyDictionary<string, string> Load(string language)
    {
        using var stream = typeof(TextCatalog).Assembly.GetManifestResourceStream($"ModuleXmlReader.Presentation.Localization.{language}.json")
            ?? throw new InvalidOperationException("Missing embedded translation resource: " + language);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)!;
    }
}

