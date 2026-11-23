using System.Diagnostics.CodeAnalysis;
using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Application.Services;
using ModuleXmlReader.Presentation.Localization;
using ModuleXmlReader.Domain;
using ModuleXmlReader.Presentation.Commands;
using ModuleXmlReader.Presentation.Services;
namespace ModuleXmlReader.Presentation.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    public TextCatalog Text { get; }
    public SimulationViewModel Simulation { get; }
    public PointNavigationViewModel Navigation { get; }
    private readonly IAppLogger _log;
    private readonly IChangelogReader? _changesReader;
    private readonly ILogReader? _logsReader;
    private IReadOnlyList<ChangeEntry> _changes = Array.Empty<ChangeEntry>();
    private IReadOnlyList<ChangelogEntryViewModel> _changeEntries = Array.Empty<ChangelogEntryViewModel>();
    public IReadOnlyList<ChangelogEntryViewModel> ChangeEntries { get => _changeEntries; private set => SetProperty(ref _changeEntries, value); }
    private string _selectedPage = "editor", _logContent = "", _logText = "";
    private bool _sidebarExpanded = true;
    private LogKind _logKind;
    public bool IsSidebarExpanded { get => _sidebarExpanded; private set { if (SetProperty(ref _sidebarExpanded, value)) { RaisePropertyChanged(nameof(SidebarWidth)); RaisePropertyChanged(nameof(SidebarLabelOpacity)); } } }
    public double SidebarWidth => IsSidebarExpanded ? 228 : 72;
    public double SidebarLabelOpacity => IsSidebarExpanded ? 1 : 0;
    public bool IsEditorPage => _selectedPage == "editor";
    public bool IsChangesPage => _selectedPage == "changes";
    public bool IsLogsPage => _selectedPage == "logs";
    public bool IsAboutPage => _selectedPage == "about";
    public string PageTitle => Text[IsEditorPage ? "Editor" : IsChangesPage ? "Changelog" : IsLogsPage ? "Logs" : "About"];
    public string PageSubtitle => Text[IsEditorPage ? "EditorSubtitle" : IsChangesPage ? "ChangelogSubtitle" : IsLogsPage ? "LogsSubtitle" : "AboutSubtitle"];
    public string LanguageButtonText => Text.Language == "ru" ? "RUS / eng" : "rus / ENG";
    public bool IsActivityLog => _logKind == LogKind.Activity;
    public bool IsErrorLog => _logKind == LogKind.Errors;
    public bool IsDiagnosticLog => _logKind == LogKind.Diagnostics;
    public string FileStatus => IsLoaded ? OpenedFilePath : Text["NoFile"];
    public string LogDirectory => _logsReader?.DirectoryPath ?? "";
    public string LogText { get => _logText; private set => SetProperty(ref _logText, value); }
    public bool HasChangelog => ChangeEntries.Count > 0;
    public string AboutText => Text["AboutText"];
    public RelayCommand ToggleSidebarCommand { get; }
    public RelayCommand ToggleLanguageCommand { get; }
    public RelayCommand ShowEditorCommand { get; }
    public RelayCommand ShowAboutPageCommand { get; }
    public AsyncCommand ShowChangesCommand { get; }
    public AsyncCommand ShowLogsCommand { get; }
    public AsyncCommand RefreshChangesCommand { get; }
    public AsyncCommand RefreshLogsCommand { get; }
    public AsyncCommand ActivityLogsCommand { get; }
    public AsyncCommand ErrorLogsCommand { get; }
    public AsyncCommand DiagnosticLogsCommand { get; }
    private readonly IModuleEditorService _editor;
    private readonly IFileDialogService _files;
    private readonly INotificationService _notifications;
    private readonly IUnsavedChangesService _unsaved;
    private readonly ISaveReviewService? _saveReview;
    private bool _refreshing, _isBusy, _isLoaded, _hasChanges;
    private string _pointTableData = "", _newScanABCDCode = "", _scanABCDCode = "";
    private string _missingPixels = "", _validationError = "", _openedFilePath = "";
    private string _rotationLabel = "Поворот: —";
    public string RotationLabel { get => _rotationLabel; private set => SetProperty(ref _rotationLabel, value); }
    private PreviewData? _preview;
    public IReadOnlyList<ParameterViewModel> Parameters { get; }
    [AllowNull]
    public string PointTableData { get => _pointTableData; set { if (SetProperty(ref _pointTableData, value ?? "")) EditorsChanged(); } }
    [AllowNull]
    public string NewScanABCDCode { get => _newScanABCDCode; set { if (SetProperty(ref _newScanABCDCode, value ?? "")) EditorsChanged(); } }
    [AllowNull]
    public string ScanABCDCode { get => _scanABCDCode; set { if (SetProperty(ref _scanABCDCode, value ?? "")) EditorsChanged(); } }
    public string MissingPixels { get => _missingPixels; private set => SetProperty(ref _missingPixels, value); }
    public string ValidationError { get => _validationError; private set => SetProperty(ref _validationError, value); }
    public string OpenedFilePath { get => _openedFilePath; private set => SetProperty(ref _openedFilePath, value); }
    public PreviewData? Preview { get => _preview; private set => SetProperty(ref _preview, value); }
    public bool IsLoaded { get => _isLoaded; private set => SetProperty(ref _isLoaded, value); }
    public bool HasChanges { get => _hasChanges; private set => SetProperty(ref _hasChanges, value); }
    public bool IsBusy { get => _isBusy; private set => SetProperty(ref _isBusy, value); }
    public bool IsEditorReadOnly => !IsLoaded || IsBusy;
    public AsyncCommand UndoCommand { get; }
    public AsyncCommand RedoCommand { get; }
    public string ActionHistory => string.Join(Environment.NewLine,_editor.ActionHistory.Select(a=>Text[a]));
    public AsyncCommand OpenCommand { get; }
    public AsyncCommand SaveCommand { get; }
    public AsyncCommand RotateCommand { get; }
    public AsyncCommand AboutCommand { get; }
    private bool CanChangeDocument => !IsBusy && IsLoaded && ValidationError.Length == 0;

    public MainWindowViewModel(IModuleEditorService editor, IFileDialogService files,
        INotificationService notifications, IAboutDialogService about, IUnsavedChangesService unsaved,
        TextCatalog? text = null, IChangelogReader? changes = null, ILogReader? logs = null, IAppLogger? logger = null, IAnimationClock? animationClock = null, IModuleComparisonService? comparison = null, ISaveReviewService? saveReview = null, IModuleBatchService? batch = null, IFolderDialogService? folders = null)
    {
        Text = text ?? new TextCatalog(); Navigation = new(Text);
        Navigation.PropertyChanged+=(_,e)=> { if(e.PropertyName==nameof(Navigation.RequestVersion)) SelectedCoordinate=Coordinates.FirstOrDefault(p=>p.Number==Navigation.SelectedNumber); }; Simulation = new SimulationViewModel(Text, animationClock ?? new ManualAnimationClock()); _log = logger ?? NullAppLogger.Instance;
        _changesReader = changes; _logsReader = logs;
        ToggleSidebarCommand = new(() => IsSidebarExpanded = !IsSidebarExpanded);
        ToggleLanguageCommand = new(Text.Toggle);
        ShowEditorCommand = new(() => Navigate("editor"));
        ShowAboutPageCommand = new(() => Navigate("about"));
        ShowChangesCommand = new(() => GuardAsync(async () => { Navigate("changes"); await LoadChangesAsync(); }), () => !IsBusy);
        ShowLogsCommand = new(() => GuardAsync(async () => { Navigate("logs"); await LoadLogsAsync(); }), () => !IsBusy);
        RefreshChangesCommand = new(() => GuardAsync(LoadChangesAsync), () => !IsBusy);
        RefreshLogsCommand = new(() => GuardAsync(LoadLogsAsync), () => !IsBusy);
        ActivityLogsCommand = new(() => SelectLogsAsync(LogKind.Activity), () => !IsBusy);
        ErrorLogsCommand = new(() => SelectLogsAsync(LogKind.Errors), () => !IsBusy);
        DiagnosticLogsCommand = new(() => SelectLogsAsync(LogKind.Diagnostics), () => !IsBusy);
        Text.LanguageChanged += LanguageChanged;
        _batch=batch; _folders=folders;
        BatchValidateCommand=new(()=>GuardAsync(()=>RunBatchAsync(false)),()=>!IsBusy&&_batch is not null);
        BatchVariantsCommand=new(()=>GuardAsync(()=>RunBatchAsync(true)),()=>!IsBusy&&_batch is not null);
        CancelBatchCommand=new(()=>_batchCancellation?.Cancel());
        _saveReview=saveReview; _comparison = comparison;
        CompareCommand = new(()=>GuardAsync(CompareAsync),()=>!IsBusy && IsLoaded && _comparison is not null);
        _editor = editor; _files = files; _notifications = notifications; _unsaved = unsaved;
        Parameters = Array.AsReadOnly(ModuleParameters.Names.Select(name => new ParameterViewModel(name, editor.State.Parameters[name])).ToArray());
        UndoCommand=new(()=>GuardAsync(()=> { _editor.Undo();return Task.CompletedTask; }),()=>!IsBusy && _editor.CanUndo);
        RedoCommand=new(()=>GuardAsync(()=> { _editor.Redo();return Task.CompletedTask; }),()=>!IsBusy && _editor.CanRedo);
        OpenCommand = new(() => GuardAsync(OpenAsync), () => !IsBusy);
        SaveCommand = new(() => GuardAsync(async () => { await SaveCoreAsync(); }), () => CanChangeDocument);
        RotateCommand = new(() => GuardAsync(() => RotateAsync(Rotation.Right90)), () => CanChangeDocument);
        AboutCommand = new(() => GuardAsync(about.ShowAboutAsync), () => !IsBusy);
        Refresh();
    }
    private void EditorsChanged()
    {
        if (_refreshing) return;
        _editor.UpdateTables(PointTableData, NewScanABCDCode, ScanABCDCode);
        Refresh();
    }
    private async Task OpenAsync()
    {
        var path = await _files.PickOpenFileAsync();
        if (path is null || !await CanDiscardAsync()) return;
        _editor.Open(path);
        await CompleteAsync();
    }
    private async Task<bool> SaveCoreAsync()
    {
        if(_saveReview is not null && !await _saveReview.ConfirmSaveAsync(new SaveReview(_editor.GetSaveDifferences())))return false;
        var path = await _files.PickSaveFileAsync();
        if (path is null) return false;
        bool saved = _editor.Save(path);
        await CompleteAsync();
        return saved;
    }
    private Task RotateAsync(Rotation rotation)
    {
        _editor.Rotate(rotation);
        return CompleteAsync();
    }
    private async Task<bool> CanDiscardAsync()
    {
        if (!_editor.State.HasChanges) return true;
        return await _unsaved.ConfirmUnsavedChangesAsync() switch
        {
            UnsavedChangesChoice.Discard => true,
            UnsavedChangesChoice.Save => await SaveCoreAsync(),
            _ => false
        };
    }
    public async Task<bool> RequestCloseAsync()
    {
        if (IsBusy) return false;
        bool result = false;
        await GuardAsync(async () => { result = await CanDiscardAsync(); });
        return result;
    }
    private async Task GuardAsync(Func<Task> action)
    {
        if (IsBusy) return;
        Simulation.Pause();
        IsBusy = true; UpdateCommands();
        try { await action(); }
        catch (Exception ex) { _log.Error("UI operation failed", ex); await _notifications.ShowAsync(new Notice(ex.Message, "Ошибка", Messages.FromException(ex))); }
        finally { IsBusy = false; Refresh(); }
    }
    private async Task CompleteAsync()
    {
        Refresh();
        var notices = _editor.State.Notices.ToArray();
        _editor.State.Notices.Clear();
        foreach (var notice in notices) await _notifications.ShowAsync(notice);
    }
    private void Refresh()
    {
        _refreshing = true;
        try
        {
            var state = _editor.State;
            foreach (var parameter in Parameters) parameter.Update(state.Parameters[parameter.Name]);
            PointTableData = state.PointTableData; NewScanABCDCode = state.NewScanABCDCode; ScanABCDCode = state.ScanABCDCode;
            MissingPixels = state.MissingPixels switch
            {
                "Загрузите файл модуля" => Text["LoadPrompt"], "Исправьте данные таблицы." => Text["InvalidTable"],
                "Не найдено пропущенных пикселей." => Text["NoMissing"], _ => state.MissingPixels
            };
            ValidationError = state.ValidationProblem is { } problem ? Text.Format(problem) : state.ValidationError;
            Preview = state.Preview; RefreshComparison(); Navigation.Update(Preview,state.PointTableData);
            Simulation.UpdateModel(state.Preview?.Simulation); RefreshCoordinates(); RefreshDiagnostics(); RefreshScanEditors();
            RotationLabel = !state.IsLoaded ? Text["RotationNone"] : state.RotationDegrees is int angle
                ? Text.Format("Rotation", angle) : Text.Format("RotationRelative", state.RelativeRotationDegrees);
            IsLoaded = state.IsLoaded; HasChanges = state.HasChanges; OpenedFilePath = state.OpenedFilePath; RaisePropertyChanged(nameof(FileStatus));
        }
        finally { _refreshing = false; }
        RaisePropertyChanged(nameof(ActionHistory));
        UpdateCommands();
    }
    private void UpdateCommands()
    {
        // Raise the derived read-only property as busy/load state changes.
        OnEditorReadOnlyChanged();
        foreach (var command in new[] { BatchValidateCommand, BatchVariantsCommand, CompareCommand, UndoCommand, RedoCommand, OpenCommand, SaveCommand, RotateCommand, AboutCommand, ShowChangesCommand, ShowLogsCommand, RefreshChangesCommand, RefreshLogsCommand, ActivityLogsCommand, ErrorLogsCommand, DiagnosticLogsCommand })
            command.RaiseCanExecuteChanged();
    }
    private void OnEditorReadOnlyChanged() => RaisePropertyChanged(nameof(IsEditorReadOnly));
    private void Navigate(string page)
    {
        if (page != "editor") Simulation.Pause();
        _selectedPage = page;
        foreach (var name in new[] { nameof(IsEditorPage), nameof(IsChangesPage), nameof(IsLogsPage), nameof(IsAboutPage), nameof(PageTitle), nameof(PageSubtitle) }) RaisePropertyChanged(name);
        _log.Debug("Navigation: " + page);
    }
    private async Task LoadChangesAsync()
    {
        _changes = await Task.Run(() => _changesReader?.Read() ?? Array.Empty<ChangeEntry>());
        RebuildChanges(); _log.Debug($"Changelog loaded: {_changes.Count} entries");
    }
    private void RebuildChanges()
    {
        ChangeEntries = _changes.Select(entry => new ChangelogEntryViewModel(entry, Text)).ToArray();
        RaisePropertyChanged(nameof(HasChangelog));
    }
    private Task SelectLogsAsync(LogKind kind) => GuardAsync(async () =>
    {
        _logKind = kind;
        RaisePropertyChanged(nameof(IsActivityLog)); RaisePropertyChanged(nameof(IsErrorLog)); RaisePropertyChanged(nameof(IsDiagnosticLog));
        await LoadLogsAsync();
    });
    private async Task LoadLogsAsync()
    {
        _logContent = await Task.Run(() => _logsReader?.ReadTail(_logKind) ?? "");
        LogText = _logContent.Length == 0 ? Text["NoLogs"] : _logContent;
    }
    private void LanguageChanged(object? sender, EventArgs e)
    {
        Refresh(); RebuildChanges();RaisePropertyChanged(nameof(BatchReport));
        LogText = _logContent.Length == 0 ? Text["NoLogs"] : _logContent;
        foreach (var name in new[] { nameof(PageTitle), nameof(PageSubtitle), nameof(LanguageButtonText), nameof(AboutText) }) RaisePropertyChanged(name);
        _log.Info("UI language changed: " + Text.Language);
    }
    public void Dispose() { Text.LanguageChanged -= LanguageChanged; Simulation.Dispose(); }

}
