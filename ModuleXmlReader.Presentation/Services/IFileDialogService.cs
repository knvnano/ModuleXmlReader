namespace ModuleXmlReader.Presentation.Services;
public interface IFileDialogService
{
    Task<string?> PickOpenFileAsync();
    Task<string?> PickSaveFileAsync();
}
