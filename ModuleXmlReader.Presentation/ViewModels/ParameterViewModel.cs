namespace ModuleXmlReader.Presentation.ViewModels;

public sealed class ParameterViewModel(string name, string value) : ObservableObject
{
    private string _value = value;
    public string Name { get; } = name;
    public string Label => Name + ":";
    public string Value { get => _value; private set => SetProperty(ref _value, value); }
    public void Update(string value) => Value = value;
}
