using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Localization;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed class ScanCodeRowViewModel : ObservableObject
{
 public int Index { get; }
 public TextCatalog Text { get; }
 public bool IsActive { get; }
 public string Background => IsActive ? "#203A4B" : "#121D30";
 public string State => Text[_invalid ? "CoordinateInvalid" : IsActive?"ActiveCode":"PaddingCode"];
 private string _value;private bool _invalid;
 public string Value {get=>_value;set=>SetProperty(ref _value,value);}
 public RelayCommand ApplyCommand {get;}
 public RelayCommand UpCommand {get;}
 public RelayCommand DownCommand {get;}
 public ScanCodeRowViewModel(int index,int value,int active,TextCatalog text,Action<int,int> apply,Action<int,int> move)
 {
  Index=index;_value=value.ToString();Text=text;IsActive=index<active;
  ApplyCommand=new(()=> { _invalid=!byte.TryParse(Value,out var n);RaisePropertyChanged(nameof(State));if(!_invalid)apply(Index,n); });
  UpCommand=new(()=> {if(Index>0&&IsActive)move(Index,-1);});
  DownCommand=new(()=> {if(Index+1<active)move(Index,1);});
 }
}
