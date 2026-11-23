using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Infrastructure;
public sealed class ModuleBatchService(Func<IModuleEditorService> createEditor) : IModuleBatchService
{
 public IReadOnlyList<BatchResult> Process(string inputFolder,string? outputFolder,bool variants,CancellationToken cancellation=default)
 {
  var files=Directory.GetFiles(inputFolder,"*.module",SearchOption.TopDirectoryOnly).Order(StringComparer.OrdinalIgnoreCase).ToArray();
  var result=new List<BatchResult>();string? destination=null;
  if(variants)
  {
   if(string.IsNullOrWhiteSpace(outputFolder))throw new ArgumentException("Output folder is required");
   destination=Path.Combine(outputFolder,"module-variants-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")+"-"+Guid.NewGuid().ToString("N")[..8]);Directory.CreateDirectory(destination);
  }
  foreach(var file in files)
  {
   if(cancellation.IsCancellationRequested) { result.Add(new(file,null,false,"",new("BatchCancelled","Cancelled")));continue; }
   try
   {
    var editor=createEditor();if(!editor.Open(file))throw new FormatException(editor.State.Notices.LastOrDefault()?.Text??"Load failed");
    var issues=new ModuleDiagnosticService().Analyze(editor.State);var errors=issues.Where(i=>i.Severity=="SeverityError").ToArray();
    if(errors.Length>0) { result.Add(new(file,null,false,"",new("BatchErrors","Validation errors: {0}",errors.Length)));continue; }
    if(!variants) {result.Add(new(file,null,true,"",new("BatchValid","Validated; findings: {0}",issues.Count)));continue;}
    for(int i=0;i<4;i++)
    {
     if(cancellation.IsCancellationRequested) {result.Add(new(file,i*90,false,"",new("BatchCancelled","Cancelled")));break;}
     if(i>0)
     {
      editor.State.Notices.Clear();editor.Rotate(Rotation.Right90);
      if(editor.State.Notices.Count>0)throw new FormatException(editor.State.Notices[^1].Text);
     }
     string target=Path.Combine(destination!,Path.GetFileNameWithoutExtension(file)+$"_plus{i*90}.module");
     if(!editor.Save(target))throw new IOException(editor.State.Notices.LastOrDefault()?.Text??"Save failed");
     result.Add(new(file,i*90,true,target,new("BatchWritten","Variant saved")));
    }
   }
   catch(Exception ex) when(ex is not OutOfMemoryException) {result.Add(new(file,null,false,"",new("BatchFailed",ex.Message,ex.Message)));}
  }
  if(files.Length==0)result.Add(new(inputFolder,null,true,"",new("BatchEmpty","No module files")));
  return result.AsReadOnly();
 }
}
