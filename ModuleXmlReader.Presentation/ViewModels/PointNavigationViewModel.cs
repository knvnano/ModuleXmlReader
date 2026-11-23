using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Localization;
namespace ModuleXmlReader.Presentation.ViewModels;
public sealed class PointNavigationViewModel : ObservableObject
{
    public TextCatalog Text { get; }
    private PreviewData? _data;
    private string _raw = "", _number = "1", _error = "";
    private int _selected, _start, _end, _version;
    public string Number { get => _number; set => SetProperty(ref _number,value); }
    public string Error => _error.Length == 0 ? "" : Text[_error];
    public int SelectedNumber => _selected;
    public int SelectionStart => _start;
    public int SelectionEnd => _end;
    public int RequestVersion => _version;
    public IReadOnlyList<PreviewPoint> Locations { get; private set; } = Array.Empty<PreviewPoint>();
    public RelayCommand GoCommand { get; }
    public PointNavigationViewModel(TextCatalog text)
    {
        Text=text; GoCommand=new(Go);
    }
    public void Update(PreviewData? data,string raw)
    {
        if (ReferenceEquals(data,_data) && raw==_raw) return;
        _data=data; _raw=raw; Locations=Array.Empty<PreviewPoint>(); _selected=0;
        RaisePropertyChanged(nameof(SelectedNumber)); RaisePropertyChanged(nameof(Locations));
    }
    public void Go()
    {
        var tokens=_raw.Split('-');
        if (_data is null || !int.TryParse(Number,out int n) || n<1 || n>tokens.Length/2)
        { _error="PointNotFound"; RaisePropertyChanged(nameof(Error)); return; }
        _error=""; _selected=n;
        _start=tokens.Take((n-1)*2).Sum(t=>t.Length+1);
        _end=_start+tokens[(n-1)*2].Length+1+tokens[(n-1)*2+1].Length;
        Locations=_data.Pixels?.Where(p=>p.Number==n).Select(p=>new PreviewPoint(p.X,p.Y)).ToArray() ?? [];
        _version++;
        foreach(var p in new[]{nameof(Error),nameof(SelectedNumber),nameof(SelectionStart),nameof(SelectionEnd),nameof(Locations),nameof(RequestVersion)}) RaisePropertyChanged(p);
    }
}
