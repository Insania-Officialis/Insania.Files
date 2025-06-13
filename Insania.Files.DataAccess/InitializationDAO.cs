using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Npgsql;

using Insania.Shared.Contracts.DataAccess;
using Insania.Shared.Contracts.Services;

using Insania.Files.Database.Contexts;
using Insania.Files.Entities;
using Insania.Files.Models.Settings;

using File = System.IO.File;

using ErrorMessagesShared = Insania.Shared.Messages.ErrorMessages;
using InformationMessages = Insania.Shared.Messages.InformationMessages;

using ErrorMessagesFiles = Insania.Files.Messages.ErrorMessages;
using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.DataAccess;

/// <summary>
/// Сервис инициализации данных в бд файлов
/// </summary>
/// <param cref="ILogger{InitializationDAO}" name="logger">Сервис логгирования</param>
/// <param cref="FilesContext" name="filesContext">Контекст базы данных файлов</param>
/// <param cref="LogsApiFilesContext" name="logsApiFilesContext">Контекст базы данных логов сервиса файлов</param>
/// <param cref="IOptions{InitializationDataSettings}" name="settings">Параметры инициализации данных</param>
/// <param cref="ITransliterationSL" name="transliteration">Сервис транслитерации</param>
/// <param cref="IConfiguration" name="configuration">Конфигурация приложения</param>
public class InitializationDAO(ILogger<InitializationDAO> logger, FilesContext filesContext, LogsApiFilesContext logsApiFilesContext, IOptions<InitializationDataSettings> settings, ITransliterationSL transliteration, IConfiguration configuration) : IInitializationDAO
{
    #region Поля
    private readonly string _username = "initializer";
    #endregion

    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<InitializationDAO> _logger = logger;

    /// <summary>
    /// Контекст базы данных файлов
    /// </summary>
    private readonly FilesContext _filesContext = filesContext;

    /// <summary>
    /// Контекст базы данных логов сервиса файлов
    /// </summary>
    private readonly LogsApiFilesContext _logsApiFilesContext = logsApiFilesContext;

    /// <summary>
    /// Параметры инициализации данных
    /// </summary>
    private readonly IOptions<InitializationDataSettings> _settings = settings;

    /// <summary>
    /// Сервис транслитерации
    /// </summary>
    private readonly ITransliterationSL _transliteration = transliteration;

    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    private readonly IConfiguration _configuration = configuration;
    #endregion

    #region Методы
    /// <summary>
    /// Метод инициализации данных
    /// </summary>
    /// <exception cref="Exception">Исключение</exception>
    public async Task Initialize()
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredInitializeMethod);

            //Инициализация структуры
            if (_settings.Value.InitStructure == true)
            {
                //Логгирование
                _logger.LogInformation("{text}", InformationMessages.InitializationStructure);

                //Инициализация баз данных в зависимости от параметров
                if (_settings.Value.Databases?.Files == true)
                {
                    //Формирование параметров
                    string connectionServer = _configuration.GetConnectionString("FilesSever") ?? throw new Exception(ErrorMessagesShared.EmptyConnectionString);
                    string patternDatabases = @"^databases_files_\d+\.sql$";
                    string connectionDatabase = _configuration.GetConnectionString("FilesEmpty") ?? throw new Exception(ErrorMessagesShared.EmptyConnectionString);
                    string patternSchemes = @"^schemes_files_\d+\.sql$";

                    //Создание базы данных
                    await CreateDatabase(connectionServer, patternDatabases, connectionDatabase, patternSchemes);
                }
                if (_settings.Value.Databases?.LogsApiFiles == true)
                {
                    //Формирование параметров
                    string connectionServer = _configuration.GetConnectionString("LogsApiFilesServer") ?? throw new Exception(ErrorMessagesShared.EmptyConnectionString);
                    string patternDatabases = @"^databases_logs_api_files_\d+\.sql$";
                    string connectionDatabase = _configuration.GetConnectionString("LogsApiFilesEmpty") ?? throw new Exception(ErrorMessagesShared.EmptyConnectionString);
                    string patternSchemes = @"^schemes_logs_api_files_\d+\.sql$";

                    //Создание базы данных
                    await CreateDatabase(connectionServer, patternDatabases, connectionDatabase, patternSchemes);
                }

                //Выход
                return;
            }

            //Накат миграций
            if (_filesContext.Database.IsRelational()) await _filesContext.Database.MigrateAsync();
            if (_logsApiFilesContext.Database.IsRelational()) await _logsApiFilesContext.Database.MigrateAsync();

            //Проверки
            if (string.IsNullOrWhiteSpace(_settings.Value.ScriptsPath)) throw new Exception(ErrorMessagesShared.EmptyScriptsPath);

            //Инициализация данных в зависимости от параметров
            if (_settings.Value.Tables?.FilesTypes == true)
            {
                //Открытие транзакции
                IDbContextTransaction transaction = _filesContext.Database.BeginTransaction();

                try
                {
                    //Создание коллекции сущностей
                    List<FileType> entities =
                    [
                        new(_transliteration, 1, _username, "Расы", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles"),
                        new(_transliteration, 2, _username, "Нации", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles"),
                        new(_transliteration, 3, _username, "Удалённый", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles", DateTime.UtcNow),
                        new(_transliteration, 4, _username, "Страны", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles"),
                        new(_transliteration, 5, _username, "Фракции", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles"),
                    ];

                    //Проход по коллекции сущностей
                    foreach (var entity in entities)
                    {
                        //Добавление сущности в бд при её отсутствии
                        if (!_filesContext.FilesTypes.Any(x => x.Id == entity.Id)) await _filesContext.FilesTypes.AddAsync(entity);
                    }

                    //Сохранение изменений в бд
                    await _filesContext.SaveChangesAsync();

                    //Создание шаблона файла скриптов
                    string pattern = @"^t_file_types_\d+.sql";

                    //Проходим по всем скриптам
                    foreach (var file in Directory.GetFiles(_settings.Value.ScriptsPath!).Where(x => Regex.IsMatch(Path.GetFileName(x), pattern)))
                    {
                        //Выполняем скрипт
                        await ExecuteScript(file, _filesContext);
                    }

                    //Фиксация транзакции
                    transaction.Commit();
                }
                catch (Exception)
                {
                    //Откат транзакции
                    transaction.Rollback();

                    //Проброс исключения
                    throw;
                }
            }
            if (_settings.Value.Tables?.Files == true)
            {
                //Открытие транзакции
                IDbContextTransaction transaction = _filesContext.Database.BeginTransaction();

                try
                {
                    //Создание коллекции ключей
                    string[][] keys =
                    [
                        ["1", "race_0.png", "1", "1", ""],
                        ["2", "race_1.png", "1", "1", ""],
                        ["3", "incorrect_content_type_0.png1", "1", "1", ""],
                        ["4", "deleted_type_0.png", "3", "0", ""],
                        ["5", "deleted_0.png", "3", "0", DateTime.UtcNow.ToString()],
                        ["6", "ichthyid.png", "1", "1", ""],
                        ["7", "mraat.png", "1", "4", ""],
                        ["8", "human.png", "1", "5", ""],
                        ["9", "vampire.png", "1", "6", ""],
                        ["10", "elf.png", "1", "7", ""],
                        ["11", "metamorf.png", "1", "8", ""],
                        ["12", "orc.png", "1", "9", ""],
                        ["13", "dwarf.png", "1", "10", ""],
                        ["14", "troll.png", "1", "11", ""],
                        ["15", "goblin.png", "1", "12", ""],
                        ["16", "ogre.png", "1", "13", ""],
                        ["17", "alv.png", "1", "14", ""],
                        ["18", "antorpozavr.png", "1", "15", ""],
                        ["19", "elvin.jpg", "1", "16", ""],
                        ["20", "danu.png", "1", "17", ""],
                        ["21", "true_ichthyid.png", "2", "1", ""],
                        ["22", "rejected_ichthyid.png", "2", "2", ""],
                        //to do 23-79 идентификаторы под изображения наций
                        ["80", "alvraat_empire.png", "4", "1", ""],
                        ["81", "principality_saorsa.png", "4", "2", ""],
                        ["82", "kingdom_bergen.png", "4", "3", ""],
                        ["83", "fesgar_principality.png", "4", "4", ""],
                        ["84", "sverdensky_kaganate.png", "4", "5", ""],
                        ["85", "khanate_tavalin.png", "4", "6", ""],
                        ["86", "principality_sargib.png", "4", "7", ""],
                        ["87", "raj_bandu.png", "4", "8", ""],
                        ["88", "kingdom_norder.png", "4", "9", ""],
                        ["89", "alter_principality.png", "4", "10", ""],
                        ["90", "orliadar_confederation.png", "4", "11", ""],
                        ["91", "kingdom_udstir.png", "4", "12", ""],
                        ["92", "kingdom_vervirung.png", "4", "13", ""],
                        ["93", "destin_order.png", "4", "14", ""],
                        ["94", "free_city_liyset.png", "4", "15", ""],
                        ["95", "liscian_empire.png", "4", "16", ""],
                        ["96", "kingdom_valtir.png", "4", "17", ""],
                        ["97", "vassal_principality_gratis.png", "4", "18", ""],
                        ["98", "principality_rekta.png", "4", "19", ""],
                        ["99", "volar.png", "4", "20", ""],
                        ["100", "union_il_ladro.png", "4", "21", ""],
                        ["102", "government.png", "5", "2", ""],
                        ["103", "aristocracy.png", "5", "3", ""],
                        ["104", "clergy.png", "5", "4", ""],
                        ["105", "magicians.png", "5", "5", ""],
                        ["106", "military.png", "5", "6", ""],
                        ["107", "merchants.png", "5", "7", ""],
                        ["108", "criminality.png", "5", "8", ""],
                        ["109", "intelligentsia.png", "5", "9", ""],
                        ["110", "factionless.png", "5", "10", ""],
                    ];

                    //Проход по коллекции ключей
                    foreach (var key in keys)
                    {
                        //Добавление сущности в бд при её отсутствии
                        if (!_filesContext.Files.Any(x => x.Id == long.Parse(key[0])))
                        {
                            //Получение сущностей
                            FileType fileType = await _filesContext.FilesTypes.FirstOrDefaultAsync(x => x.Id == long.Parse(key[2])) ?? throw new Exception(ErrorMessagesFiles.NotFoundFileType);

                            //Создание сущности
                            DateTime? dateDeleted = null;
                            if (!string.IsNullOrWhiteSpace(key[4])) dateDeleted = DateTime.Parse(key[4]);
                            FileEntity entity = new(long.Parse(key[0]), _username, true, key[1], fileType, long.Parse(key[3]), dateDeleted);

                            //Добавление сущности в бд
                            await _filesContext.Files.AddAsync(entity);
                        }
                    }

                    //Сохранение изменений в бд
                    await _filesContext.SaveChangesAsync();

                    //Фиксация транзакции
                    transaction.Commit();
                }
                catch (Exception)
                {
                    //Откат транзакции
                    transaction.Rollback();

                    //Проброс исключения
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text}: {error}", ErrorMessagesShared.Error, ex.Message);

            //Проброс исключения
            throw;
        }
    }

    /// <summary>
    /// Метод создание базы данных
    /// </summary>
    /// <param name="connectionServer">Строка подключения к серверу</param>
    /// <param name="patternDatabases">Шаблон файлов создания базы данных</param>
    /// <param name="connectionDatabase">Строка подключения к базе данных</param>
    /// <param name="patternSchemes">Шаблон файлов создания схемы</param>
    /// <returns></returns>
    private async Task CreateDatabase(string connectionServer, string patternDatabases, string connectionDatabase, string patternSchemes)
    {
        //Проход по всем скриптам в директории и создание баз данных
        foreach (var file in Directory.GetFiles(_settings.Value.ScriptsPath!).Where(x => Regex.IsMatch(Path.GetFileName(x), patternDatabases)))
        {
            //Выполнение скрипта
            await ExecuteScript(file, connectionServer);
        }

        //Проход по всем скриптам в директории и создание схем
        foreach (var file in Directory.GetFiles(_settings.Value.ScriptsPath!).Where(x => Regex.IsMatch(Path.GetFileName(x), patternSchemes)))
        {
            //Выполнение скрипта
            await ExecuteScript(file, connectionDatabase);
        }
    }

    /// <summary>
    /// Метод выполнения скрипта со строкой подключения
    /// </summary>
    /// <param cref="string" name="filePath">Путь к скрипту</param>
    /// <param cref="string" name="connectionString">Строка подключения</param>
    private async Task ExecuteScript(string filePath, string connectionString)
    {
        //Логгирование
        _logger.LogInformation("{text} {params}", InformationMessages.ExecuteScript, filePath);

        try
        {
            //Создание соединения к бд
            using NpgsqlConnection connection = new(connectionString);

            //Открытие соединения
            connection.Open();

            //Считывание запроса
            string sql = File.ReadAllText(filePath);

            //Создание sql-запроса
            using NpgsqlCommand command = new(sql, connection);

            //Выполнение команды
            await command.ExecuteNonQueryAsync();

            //Логгирование
            _logger.LogInformation("{text} {params}", InformationMessages.ExecutedScript, filePath);
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text} {params} из-за ошибки {ex}", ErrorMessagesShared.NotExecutedScript, filePath, ex);
        }
    }

    /// <summary>
    /// Метод выполнения скрипта с контекстом
    /// </summary>
    /// <param cref="string" name="filePath">Путь к скрипту</param>
    /// <param cref="DbContext" name="context">Контекст базы данных</param>
    private async Task ExecuteScript(string filePath, DbContext context)
    {
        //Логгирование
        _logger.LogInformation("{text} {params}", InformationMessages.ExecuteScript, filePath);

        try
        {
            //Считывание запроса
            string sql = File.ReadAllText(filePath);

            //Выполнение sql-команды
            await context.Database.ExecuteSqlRawAsync(sql);

            //Логгирование
            _logger.LogInformation("{text} {params}", InformationMessages.ExecutedScript, filePath);
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text} {params} из-за ошибки {ex}", ErrorMessagesShared.NotExecutedScript, filePath, ex);
        }
    }
    #endregion
}