using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using ModuleXmlReader.Application.Abstractions;
namespace ModuleXmlReader.Infrastructure;

public sealed class Log4NetLogger : IAppLogger, IDisposable
{
    private readonly ILoggerRepository _repository;
    private readonly ILog _log;
    public string DirectoryPath { get; }
    public Log4NetLogger(string directory, string maximumSize = "2MB", int backups = 3)
    {
        DirectoryPath = Path.GetFullPath(directory); Directory.CreateDirectory(DirectoryPath);
        _repository = LogManager.CreateRepository("ModuleXmlReader-" + Guid.NewGuid().ToString("N"));
        var activity = Create("activity.log", Level.Info, Level.Warn);
        var errors = Create("errors.log", Level.Error, Level.Fatal);
        var diagnostics = Create("diagnostics.log", Level.Debug, Level.Debug);
        ((Hierarchy)_repository).Root.Level = Level.Debug;
        log4net.Config.BasicConfigurator.Configure(_repository, activity, errors, diagnostics);
        _log = LogManager.GetLogger(_repository.Name, "ModuleXmlReader");

        RollingFileAppender Create(string name, Level min, Level max)
        {
            var appender = new RollingFileAppender
            {
                Name = name, File = Path.Combine(DirectoryPath, name), AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size, MaximumFileSize = maximumSize,
                MaxSizeRollBackups = backups, StaticLogFileName = true, ImmediateFlush = true,
                Encoding = new System.Text.UTF8Encoding(false), LockingModel = new FileAppender.MinimalLock(),
                Layout = new PatternLayout("%date{ISO8601} %-5level [%thread] %logger - %message%newline%exception")
            };
            appender.AddFilter(new LevelRangeFilter { LevelMin = min, LevelMax = max });
            appender.ActivateOptions();
            return appender;
        }
    }
    public void Info(string message) => _log.Info(message);
    public void Debug(string message) => _log.Debug(message);
    public void Error(string message, Exception exception) => _log.Error(message, exception);
    public void Dispose() => _repository.Shutdown();
}
