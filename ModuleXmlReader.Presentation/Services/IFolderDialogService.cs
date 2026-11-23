namespace ModuleXmlReader.Presentation.Services;
public interface IFolderDialogService
{
 Task<string?> PickFolderAsync(bool output);
}
