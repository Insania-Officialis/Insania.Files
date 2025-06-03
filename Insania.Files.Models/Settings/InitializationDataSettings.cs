namespace Insania.Files.Models.Settings;

/// <summary>
/// Модель параметров инициализации данных
/// </summary>
public class InitializationDataSettings
{
    /// <summary>
    /// Признак инициализации структуры
    /// </summary>
    /// <remarks>
    /// Нужен для запуска миграций, при true не происходит инициализация данных
    /// </remarks>
    public bool? InitStructure { get; set; }

    /// <summary>
    /// Путь к файлам скриптов
    /// </summary>
    public string? ScriptsPath { get; set; }

    /// <summary>
    /// Включение в инициализацию таблиц
    /// </summary>
    public InitializationDataSettingsIncludeTables? Tables { get; set; }

    /// <summary>
    /// Включение в инициализацию баз данных
    /// </summary>
    public InitializationDataSettingsIncludeDatabases? Databases { get; set; }
}

/// <summary>
/// Модель параметра включения в инициализацию таблиц
/// </summary>
public class InitializationDataSettingsIncludeTables
{
    /// <summary>
    /// Типы файлов
    /// </summary>
    public bool? FilesTypes { get; set; }

    /// <summary>
    /// Файлы
    /// </summary>
    public bool? Files { get; set; }
}

/// <summary>
/// Модель параметра включения в инициализацию баз данных
/// </summary>
public class InitializationDataSettingsIncludeDatabases
{
    /// <summary>
    /// Файлы
    /// </summary>
    public bool? Files { get; set; }

    /// <summary>
    /// Логи сервиса файлов
    /// </summary>
    public bool? LogsApiFiles { get; set; }
}