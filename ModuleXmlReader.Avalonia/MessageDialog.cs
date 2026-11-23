using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Localization;
namespace ModuleXmlReader.Avalonia;
internal static class MessageDialog
{
    public static async Task Show(Window owner, Notice notice, TextCatalog text)
    {
        var dialog = new Window { Width = 560, SizeToContent = SizeToContent.Height, CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner };
        var label = new TextBlock { TextWrapping = global::Avalonia.Media.TextWrapping.Wrap };
        var ok = new Button { HorizontalAlignment = HorizontalAlignment.Right, IsDefault = true };
        ok.Classes.Add("primary"); ok.Click += (_, _) => dialog.Close();
        dialog.Content = new StackPanel { Margin = new Thickness(24), Spacing = 20, Children = { label, ok } };
        void Update(object? sender, EventArgs e) { dialog.Title = text[notice.TitleKey]; label.Text = text.NoticeText(notice); ok.Content = text["Ok"]; }
        Update(null, EventArgs.Empty); text.LanguageChanged += Update;
        try { await dialog.ShowDialog(owner); } finally { text.LanguageChanged -= Update; }
    }
}
