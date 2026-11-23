using ModuleXmlReader.Application.Services;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed partial class MainWindowViewModel
{
 private IReadOnlyList<ScanCodeRowViewModel> _newCodes=Array.Empty<ScanCodeRowViewModel>(),_legacyCodes=Array.Empty<ScanCodeRowViewModel>();
 private string _codesSignature="";
 public IReadOnlyList<ScanCodeRowViewModel> NewCodes {get=>_newCodes;private set=>SetProperty(ref _newCodes,value);}
 public IReadOnlyList<ScanCodeRowViewModel> LegacyCodes {get=>_legacyCodes;private set=>SetProperty(ref _legacyCodes,value);}
 private void RefreshScanEditors()
 {
  var signature=NewScanABCDCode+"|"+ScanABCDCode+"|"+_editor.State.Parameters["ScanType"]+Text.Language;if(signature==_codesSignature)return;_codesSignature=signature;
  NewCodes=BuildCodes(true);LegacyCodes=BuildCodes(false);
 }
 private IReadOnlyList<ScanCodeRowViewModel> BuildCodes(bool modern)
 {
  var service=new ScanCodeService();int[] values;
  try { values=service.Parse(modern?NewScanABCDCode:ScanABCDCode,modern?"NewScanABCDCode":"ScanABCDCode"); }
  catch(FormatException) {return Array.Empty<ScanCodeRowViewModel>();}
  int count=int.TryParse(_editor.State.Parameters["ScanType"].Replace("Scan_",""),out var scan)?scan:values.Length;
  int active=Math.Min(values.Length,modern?Math.Max(0,count):16);
  return values.Select((v,i)=>new ScanCodeRowViewModel(i,v,active,Text,(index,n)=>
  {
   if(IsBusy||!IsLoaded)return;var data=service.Parse(modern?NewScanABCDCode:ScanABCDCode,"Scan");data[index]=n;SetCodes(modern,string.Join("-",data));
  },(index,offset)=>
  {
   if(IsBusy||!IsLoaded)return;SetCodes(modern,service.MoveActive(modern?NewScanABCDCode:ScanABCDCode,index,offset,active,"Scan"));
  })).ToArray();
 }
 private void SetCodes(bool modern,string value) {if(modern)NewScanABCDCode=value;else ScanABCDCode=value;}
}
