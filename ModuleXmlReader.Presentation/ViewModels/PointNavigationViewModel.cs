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
    public bool HasNoMissing => Missing.Count==0;
    public IReadOnlyList<MissingPointViewModel> Missing { get; private set; } = Array.Empty<MissingPointViewModel>();
    public RelayCommand PreviousMissingCommand { get; }
    public RelayCommand NextMissingCommand { get; }
    public RelayCommand GoCommand { get; }
    public PointNavigationViewModel(TextCatalog text)
    {
        Text=text; GoCommand=new(Go);
        PreviousMissingCommand=new(()=>MoveMissing(-1)); NextMissingCommand=new(()=>MoveMissing(1));
        Text.LanguageChanged+=(_,_)=>RaisePropertyChanged(nameof(Error));
    }
    public void Update(PreviewData? data,string raw)
    {
        if (ReferenceEquals(data,_data) && raw==_raw) return;
        _data=data; _raw=raw;
        var tokens=raw.Split('-');
        Missing=Enumerable.Range(0,tokens.Length/2).Where(i=>tokens[i*2].Trim()=="255" && tokens[i*2+1].Trim()=="255").Select(i=>new MissingPointViewModel(i+1,()=>Select(i+1))).ToArray();
        RaisePropertyChanged(nameof(Missing)); RaisePropertyChanged(nameof(HasNoMissing));
        Locations=Array.Empty<PreviewPoint>(); _selected=0;
        RaisePropertyChanged(nameof(SelectedNumber)); RaisePropertyChanged(nameof(Locations));
    }
    public void Select(int number) { Number=number.ToString(System.Globalization.CultureInfo.InvariantCulture); Go(); }
    private void MoveMissing(int direction)
    {
        if(Missing.Count==0) return;
        var index=Missing.ToList().FindIndex(p=>p.Number==SelectedNumber);
        if(index<0) index=direction>0 ? -1 : 0;
        Select(Missing[(index+direction+Missing.Count)%Missing.Count].Number);
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
        if (Locations.Count==0 && _data.GapMarkers is { } gaps)
            Locations=gaps.Where(g=>n>=g.FirstMissing && n<g.FirstMissing+g.Count).Select(g=>new PreviewPoint(g.X,g.Y)).Distinct().ToArray();
        _version++;
        foreach(var p in new[]{nameof(Error),nameof(SelectedNumber),nameof(SelectionStart),nameof(SelectionEnd),nameof(Locations),nameof(RequestVersion)}) RaisePropertyChanged(p);
    }
}

public sealed record MissingPointViewModel(int Number, Action Select)
{
    public RelayCommand SelectCommand { get; } = new(Select);
}
