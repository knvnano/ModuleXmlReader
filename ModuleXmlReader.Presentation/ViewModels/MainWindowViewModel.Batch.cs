using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Services;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed partial class MainWindowViewModel
{
 private readonly IModuleBatchService? _batch;
 private readonly IFolderDialogService? _folders;
 private CancellationTokenSource? _batchCancellation;
 private IReadOnlyList<BatchResult> _batchResults=Array.Empty<BatchResult>();
 public string BatchReport=>string.Join(Environment.NewLine,_batchResults.Select(r=>$"{r.File} {(r.RelativeAngle is int angle?$"+{angle}°":"")} → {Text.Format(r.Message)} {r.Output}"));
 public AsyncCommand BatchValidateCommand {get;}
 public AsyncCommand BatchVariantsCommand {get;}
 public RelayCommand CancelBatchCommand {get;}
 private async Task RunBatchAsync(bool variants)
 {
  if(_folders is null||_batch is null)return;
  var input=await _folders.PickFolderAsync(false);if(input is null)return;
  var output=variants?await _folders.PickFolderAsync(true):null;if(variants&&output is null)return;
  using var cancellation=new CancellationTokenSource();_batchCancellation=cancellation;
  try { _batchResults=await Task.Run(()=>_batch.Process(input,output,variants,cancellation.Token));RaisePropertyChanged(nameof(BatchReport));_log.Info($"Batch completed: {_batchResults.Count} results"); }
  finally {_batchCancellation=null;}
 }
}
