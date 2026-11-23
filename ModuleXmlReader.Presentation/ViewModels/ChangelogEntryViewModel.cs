using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Localization;
namespace ModuleXmlReader.Presentation.ViewModels;

public sealed class ChangelogEntryViewModel(ChangeEntry entry, TextCatalog text)
{
    public string Version => "v" + entry.Version;
    public string Date => entry.Date;
    public string Title => text.Language == "ru" ? entry.TitleRu : entry.TitleEn;
    public string Description => string.Join(Environment.NewLine + Environment.NewLine,
        (text.Language == "ru" ? entry.ItemsRu : entry.ItemsEn).Select(item => "• " + item));
}
