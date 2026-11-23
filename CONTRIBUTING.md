# Contributing

Use .NET SDK 10 and the `ModuleXmlReader.Avalonia.sln` solution. Build and run tests before submitting a pull request:

```powershell
dotnet build ModuleXmlReader.Avalonia.sln -c Release
dotnet test ModuleXmlReader.Avalonia.sln -c Release --no-build
```

Keep Domain and Application independent of Avalonia. Put filesystem integrations in Infrastructure and UI state in Presentation. Add Russian and English resources together. Preserve unknown XML fields and scan-table padding. Never discard zero scan addresses or invent physical coordinates for missing points.

Include a minimal reproducible module when reporting a parser or rotation issue, after removing confidential configuration and paths. Explain the expected orientation and the reference application's output. Add relevant regression tests and a changelog entry. Do not commit logs, generated exports, build outputs or credentials.
