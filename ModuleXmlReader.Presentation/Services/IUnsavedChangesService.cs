namespace ModuleXmlReader.Presentation.Services;
public enum UnsavedChangesChoice { Cancel, Save, Discard }
public interface IUnsavedChangesService
{
    Task<UnsavedChangesChoice> ConfirmUnsavedChangesAsync();
}
