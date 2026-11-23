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
            if(DataContext is MainWindowViewModel vm && e.KeyModifiers.HasFlag(global::Avalonia.Input.KeyModifiers.Control) && FocusManager?.GetFocusedElement() is not TextBox)
            {
                var command=e.Key==global::Avalonia.Input.Key.Z ? vm.UndoCommand : e.Key==global::Avalonia.Input.Key.Y ? vm.RedoCommand : null;
                if(command?.CanExecute(null)==true) { command.Execute(null);e.Handled=true;return; }
            }
            if (e.Key == global::Avalonia.Input.Key.Z && e.KeyModifiers == global::Avalonia.Input.KeyModifiers.None &&
                FocusManager?.GetFocusedElement() is not TextBox)
            { ModuleCanvas.FitToView(); e.Handled = true; }
        }, global::Avalonia.Interactivity.RoutingStrategies.Tunnel);
        DataContextChanged += (_,_) =>
        {
            if(DataContext is MainWindowViewModel model) model.Navigation.PropertyChanged += (_,e) =>
            { if(e.PropertyName=="RequestVersion") { if(DataTabs.SelectedIndex!=0) { DataTabs.SelectedIndex=1; PointTableText.Focus(); }
              else if(model.SelectedCoordinate is not null) CoordinateList.ScrollIntoView(model.SelectedCoordinate); } };
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
