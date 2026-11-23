using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Localization;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed class CoordinateRowViewModel : ObservableObject
{
 public int Number { get; }
 public TextCatalog Text { get; }
 private string _x,_y,_error="";
 public string X {get=>_x;set=>SetProperty(ref _x,value);}
 public string Y {get=>_y;set=>SetProperty(ref _y,value);}
 public string State => _error.Length>0 ? Text[_error] : X=="255" && Y=="255" ? Text["Missing"] : Text["CoordinateValid"];
 public RelayCommand ApplyCommand { get; }
 public CoordinateRowViewModel(int number,int x,int y,TextCatalog text,Func<int,int,int,bool> apply)
 {
  Number=number;_x=x.ToString();_y=y.ToString();Text=text;
  ApplyCommand=new(()=>
  {
   if(!byte.TryParse(X,out var a)||!byte.TryParse(Y,out var b)||(a==255)!=(b==255))_error="CoordinateInvalid";
   else _error=apply(Number,a,b)?"":"CoordinateInvalid";
   RaisePropertyChanged(nameof(State));
  });
 }
}
