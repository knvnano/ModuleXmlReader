# Nova XML Reader — Avalonia UI 0.5.0

Выполнены перенос на Avalonia, выделение слоистой MVVM-архитектуры и исправления
третьего этапа. Повороты проверяются по четырём предоставленным эталонам NovaLCT.
Исходный WinForms-проект `Module_XML_reader` и его решение не изменены.

Тёмная тема, мгновенный RUS/ENG, анимированное меню, changelog и раздельные журналы log4net.

[Интерфейс, перевод, история и логи](docs/INTERFACE-LOCALIZATION-LOGGING.md).

## Запуск и проверки

Требуются .NET SDK 10.0 и доступ к NuGet при первом восстановлении пакетов.

```powershell
dotnet build ModuleXmlReader.Avalonia.sln -c Release
dotnet test ModuleXmlReader.CharacterizationTests/ModuleXmlReader.CharacterizationTests.csproj -c Release --no-build
dotnet run --project ModuleXmlReader.Avalonia/ModuleXmlReader.Avalonia.csproj
```

## Проекты

- `ModuleXmlReader.Domain` — модели координат, геометрии и состояния редактора.
- `ModuleXmlReader.Application` — сценарии, валидация, повороты, коды и предпросмотр.
- `ModuleXmlReader.Infrastructure` — XML, безопасная запись файлов, внешние ссылки.
- `ModuleXmlReader.Presentation` — ViewModels и команды без зависимости от Avalonia.
- `ModuleXmlReader.Avalonia` — Views, отрисовка, диалоги, внедрение зависимостей.
- `ModuleXmlReader.CharacterizationTests` — регрессионные тесты и эталоны в Fixtures.

[Отчёт об исправлениях и анализ эталонов](docs/STAGE3-FIXES.md).
[История архитектуры второго этапа](docs/STAGE2-ARCHITECTURE.md).
[История переноса первого этапа](docs/STAGE1-COMPATIBILITY.md).

Поддерживаются равные прямоугольные группы; соответствие NovaLCT подтверждено
присланным профилем DP5135Common / Scan_32 / две группы. Для других схем нужны
дополнительные эталоны; ограничения и результаты проверок описаны в отчёте.

