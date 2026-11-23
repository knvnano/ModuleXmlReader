using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Application.Services;

public static class Messages
{
    public static FormatException Format(string key, string fallback, params object[] args)
    {
        var ex = new FormatException(fallback); ex.Data[nameof(MessageToken)] = new MessageToken(key, fallback, args); return ex;
    }
    public static MessageToken FromException(Exception ex) => ex.Data[nameof(MessageToken)] as MessageToken ?? ex switch
    {
        FileNotFoundException missing => new("FileMissing", ex.Message, missing.FileName ?? ""),
        UnauthorizedAccessException => new("AccessDenied", ex.Message),
        IOException => new("IoFailure", ex.Message),
        _ => new("OperationFailed", ex.Message)
    };
}
