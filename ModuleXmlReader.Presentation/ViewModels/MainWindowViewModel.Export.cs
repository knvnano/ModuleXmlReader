using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed partial class MainWindowViewModel
{
 private readonly IModuleExportWriter? _exportWriter;
 public AsyncCommand ExportCommand {get;}
 private async Task ExportAsync()
 {
  if(_folders is null||_exportWriter is null)return;
  var folder=await _folders.PickFolderAsync(true);if(folder is null)return;
  var bundle=new ModuleExportService().Build(_editor.State,key=>Text[key],token=>Text.Format(token));
  var path=await Task.Run(()=>_exportWriter.Write(folder,bundle));
  _log.Info("Module report exported: "+path);
  await _notifications.ShowAsync(new Notice(path,"Export",new("ExportDone",path,path),"Export"));
 }
}
