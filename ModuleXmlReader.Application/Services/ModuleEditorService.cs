using ModuleXmlReader.Application.Abstractions;
using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

public sealed class ModuleEditorService(IModuleRepository repository, IMissingPixelService missingPixels,
    IModuleRotationService rotations, IModuleValidationService validation, IPreviewService preview, IAppLogger? logger = null, IModuleSimulationService? simulation = null) : IModuleEditorService
{
    private readonly IModuleSimulationService _simulation = simulation ?? new ModuleSimulationService();
    private readonly IAppLogger _log = logger ?? NullAppLogger.Instance;
    private IModuleDocument? _document;
    private readonly List<(ModuleEditorState State,string Action)> _undo = new(), _redo = new();
    private readonly List<string> _actions = new();
    public bool CanUndo => _undo.Count>0;
    public bool CanRedo => _redo.Count>0;
    public IReadOnlyList<string> ActionHistory => _actions.AsReadOnly();
    private void Remember(ModuleEditorState before,string action)
    {
        if(Signature(before)==Signature(State)) return;
        _undo.Add((before,action)); if(_undo.Count>200)_undo.RemoveAt(0); _redo.Clear();
        _actions.Add(action); if(_actions.Count>200)_actions.RemoveAt(0);
    }
    public void Undo() => Travel(_undo,_redo,"ActionUndo");
    public void Redo() => Travel(_redo,_undo,"ActionRedo");
    private void Travel(List<(ModuleEditorState State,string Action)> from,List<(ModuleEditorState State,string Action)> to,string action)
    {
        if(from.Count==0)return;
        var item=from[^1]; from.RemoveAt(from.Count-1); to.Add((CopyState(),item.Action));
        var path=State.OpenedFilePath; Apply(item.State); State.OpenedFilePath=path;
        State.HasChanges=Signature(State)!=_savedSignature; _actions.Add(action);
        if(_actions.Count>200)_actions.RemoveAt(0);
    }
    private string _savedSignature = string.Empty;
    public ModuleEditorState State { get; } = new();

    public bool Open(string filePath)
    {
        try
        {
            _log.Info($"Opening module: {filePath}");
            var document = repository.Load(filePath);
            if (!document.HasModule) throw Messages.Format("ModuleAbsent", "Не найден элемент Config/Module.");
            var candidate = new ModuleEditorState { OpenedFilePath = filePath };
            foreach (var name in ModuleParameters.Names) candidate.Parameters[name] = document.GetValue(name);
            candidate.PointTableData = document.GetValue("PointTableData") ?? string.Empty;
            candidate.ScanABCDCode = document.GetValue("ScanABCDCode") ?? string.Empty;
            candidate.NewScanABCDCode = document.GetValue("NewScanABCDCode") ?? string.Empty;
            RefreshDerived(candidate);
            candidate.RotationDegrees = rotations.DetectOrientation(candidate);
            _document = document;
            Apply(candidate);
            _savedSignature = Signature(State);
            _undo.Clear(); _redo.Clear(); _actions.Clear();
            State.HasChanges = false;
            _log.Info($"Module opened: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            _log.Error($"Module open failed: {filePath}", ex);
            Notify("Ошибка при загрузке файла: " + ex.Message, "Ошибка", new("OpenFailed", ex.Message, Messages.FromException(ex)));
            return false;
        }
    }
    public bool Save(string filePath)
    {
        try
        {
            if (_document is null) throw new InvalidOperationException("Сначала откройте файл модуля.");
            RefreshDerived(State);
            var document = _document.Clone();
            foreach (var name in ModuleParameters.Names)
                if (State.Parameters[name].Length > 0) document.SetValue(name, State.Parameters[name]);
            document.SetValue("PointTableData", State.PointTableData);
            document.SetValue("ScanABCDCode", State.ScanABCDCode);
            document.SetValue("NewScanABCDCode", State.NewScanABCDCode);
            repository.Save(document, filePath);
            _document = document;
            State.OpenedFilePath = filePath;
            _savedSignature = Signature(State);
            State.HasChanges = false;
            _log.Info($"Module saved: {filePath}");
            Notify("Файл успешно сохранён как " + filePath, "Сохранение", new("SaveSuccess", filePath, filePath));
            return true;
        }
        catch (Exception ex)
        {
            _log.Error($"Module save failed: {filePath}", ex);
            Notify("Ошибка при сохранении файла: " + ex.Message, "Ошибка", new("SaveFailed", ex.Message, Messages.FromException(ex)));
            return false;
        }
    }
    public void UpdateTables(string points, string newScan, string scan)
    {
        if (!State.IsLoaded) return;
        var before=CopyState();
        State.PointTableData = points;
        State.NewScanABCDCode = newScan;
        State.ScanABCDCode = scan;
        State.Parameters["ScanABCDCode"] = scan;
        try { RefreshDerived(State); }
        catch (Exception ex)
        {
            State.ValidationError = ex.Message;
            State.ValidationProblem = Messages.FromException(ex);
            _log.Debug("Editor validation: " + ex.Message);
            State.Preview = null;
            State.MissingPixels = "Исправьте данные таблицы.";
        }
        State.HasChanges = Signature(State) != _savedSignature;
        Remember(before,"ActionEdit");
    }
    public void Rotate(Rotation rotation)
    {
        try
        {
            if (_document is null) throw new InvalidOperationException("Сначала откройте файл модуля.");
            var before=CopyState();
            var candidate = CopyState();
            rotations.Rotate(candidate, rotation);
            RefreshDerived(candidate);
            Apply(candidate); Remember(before,"ActionRotate");
            _log.Info($"Module rotated: {rotation}; angle={State.RotationDegrees}");
            State.HasChanges = Signature(State) != _savedSignature;
        }
        catch (Exception ex) { _log.Error("Module rotation failed", ex); Notify("Ошибка при повороте: " + ex.Message, "Ошибка", new("RotateFailed", ex.Message, Messages.FromException(ex))); }
    }
    private void RefreshDerived(ModuleEditorState state)
    {
        var module = validation.Validate(state);
        state.Preview = preview.Create(module) with { Simulation = _simulation.Create(module, state) };
        state.MissingPixels = missingPixels.CalculateDisplayText(state.PointTableData);
        state.Parameters["TotalPointInTable"] = module.Points.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        state.Parameters["ScanABCDCode"] = state.ScanABCDCode;
        state.ValidationError = string.Empty;
        state.ValidationProblem = null;
    }
    private ModuleEditorState CopyState()
    {
        var copy = new ModuleEditorState();
        Transfer(State, copy);
        return copy;
    }
    private void Apply(ModuleEditorState candidate) => Transfer(candidate, State);
    private static void Transfer(ModuleEditorState from, ModuleEditorState to)
    {
        foreach (var name in ModuleParameters.Names) to.Parameters[name] = from.Parameters[name];
        to.OpenedFilePath = from.OpenedFilePath;
        to.RotationDegrees = from.RotationDegrees;
        to.RelativeRotationDegrees = from.RelativeRotationDegrees;
        to.PointTableData = from.PointTableData;
        to.NewScanABCDCode = from.NewScanABCDCode;
        to.ScanABCDCode = from.ScanABCDCode;
        to.MissingPixels = from.MissingPixels;
        to.ValidationError = from.ValidationError;
        to.ValidationProblem = from.ValidationProblem;
        to.Preview = from.Preview;
    }
    private static string Signature(ModuleEditorState s) => string.Join('\u001f', ModuleParameters.Names
        .Select(name => s.Parameters[name]).Concat(new[] { s.PointTableData, s.ScanABCDCode, s.NewScanABCDCode }));
    private void Notify(string text, string title, MessageToken? message = null) => State.Notices.Add(new(text, title, message, title == "Сохранение" ? "Saving" : "Error"));
}
