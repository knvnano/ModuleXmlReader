using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Presentation.Services;
public interface INotificationService
{
    Task ShowAsync(string text, string title);
    Task ShowAsync(Notice notice) => ShowAsync(notice.Text, notice.Title);
}
