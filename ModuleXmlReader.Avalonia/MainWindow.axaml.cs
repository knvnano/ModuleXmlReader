using Avalonia.Controls;
using ModuleXmlReader.Presentation.ViewModels;
namespace ModuleXmlReader.Avalonia;

public partial class MainWindow : Window
{
    private bool _closeApproved;
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, (_, e) =>
        {
            if (e.Key == global::Avalonia.Input.Key.Z && e.KeyModifiers == global::Avalonia.Input.KeyModifiers.None &&
                FocusManager?.GetFocusedElement() is not TextBox)
            { ModuleCanvas.FitToView(); e.Handled = true; }
        }, global::Avalonia.Interactivity.RoutingStrategies.Tunnel);
        DataContextChanged += (_,_) =>
        {
            if(DataContext is MainWindowViewModel model) model.Navigation.PropertyChanged += (_,e) =>
            { if(e.PropertyName=="RequestVersion") { DataTabs.SelectedIndex=0; PointTableText.Focus(); } };
        };
        Closing += async (_, e) =>
        {
            if (_closeApproved || DataContext is not MainWindowViewModel vm || (!vm.HasChanges && !vm.IsBusy)) return;
            e.Cancel = true;
            if (await vm.RequestCloseAsync())
                global::Avalonia.Threading.Dispatcher.UIThread.Post(() => { _closeApproved = true; Close(); });
        };
    }
}
