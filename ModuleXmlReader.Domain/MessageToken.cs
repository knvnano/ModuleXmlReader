namespace ModuleXmlReader.Domain;

// Language-independent identity with a fallback for diagnostics and old consumers.
public sealed record MessageToken(string Key, string Fallback, params object[] Arguments);
public sealed record ChangeEntry(string Version, string Date, string TitleRu, string TitleEn,
    IReadOnlyList<string> ItemsRu, IReadOnlyList<string> ItemsEn);
public enum LogKind { Activity, Errors, Diagnostics }
