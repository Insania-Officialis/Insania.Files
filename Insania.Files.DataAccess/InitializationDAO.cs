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
                        new(_transliteration, 6, _username, "Общее", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles"),
                        new(_transliteration, 7, _username, "Новости", "E:\\Program\\Insania\\Insania.Files\\Insania.Files.Tests\\MockFiles")
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
                        ["23", "dryevniiy.png", "2", "3", ""],
                        ["24", "nag.png", "2", "4", ""],
                        ["25", "dikiiy_mraat.png", "2", "5", ""],
                        ["26", "tsivilizovannyiy_mraat.png", "2", "6", ""],
                        ["27", "listsiyets.png", "2", "7", ""],
                        ["28", "rifut.png", "2", "8", ""],
                        ["29", "lastat.png", "2", "9", ""],
                        ["30", "dyestinyets.png", "2", "10", ""],
                        ["31", "ilmariyets.png", "2", "11", ""],
                        ["32", "asud.png", "2", "12", ""],
                        ["33", "val'tiryets.png", "2", "13", ""],
                        ["34", "saorsin.png", "2", "14", ""],
                        ["35", "tyeoranyets.png", "2", "15", ""],
                        ["36", "ankostyets.png", "2", "16", ""],
                        ["37", "tavalinyets.png", "2", "17", ""],
                        ["38", "iglyessiyets.png", "2", "18", ""],
                        ["39", "plyekiyets.png", "2", "19", ""],
                        ["40", "siyervin.png", "2", "20", ""],
                        ["41", "viyegiyets.png", "2", "21", ""],
                        ["42", "zapadnyiy_vampir.png", "2", "22", ""],
                        ["43", "vostochnyiy_vampir.png", "2", "23", ""],
                        ["44", "vysshiiy_el'f.png", "2", "24", ""],
                        ["45", "nochnoiy_el'f.png", "2", "25", ""],
                        ["46", "krovavyiy_el'f.png", "2", "26", ""],
                        ["47", "lyesnoiy_el'f.png", "2", "27", ""],
                        ["48", "gornyiy_el'f.png", "2", "28", ""],
                        ["49", "ryechnoiy_el'f.png", "2", "29", ""],
                        ["50", "solnyechnyiy_el'f.png", "2", "30", ""],
                        ["51", "morskoiy_el'f.png", "2", "31", ""],
                        ["52", "volchiiy_myetamorf.png", "2", "32", ""],
                        ["53", "myedvyezhiiy_myetamorf.png", "2", "33", ""],
                        ["54", "koshachiiy_myetamorf.png", "2", "34", ""],
                        ["55", "syeryiy_ork.png", "2", "35", ""],
                        ["56", "chyornyiy_ork.png", "2", "36", ""],
                        ["57", "zyelyonyiy_ork.png", "2", "37", ""],
                        ["58", "byelyiy_ork.png", "2", "38", ""],
                        ["59", "yuzhnyiy_ork.png", "2", "39", ""],
                        ["60", "bakkyer.png", "2", "40", ""],
                        ["61", "nordyeryets.png", "2", "41", ""],
                        ["62", "vyervirungyets.png", "2", "42", ""],
                        ["63", "shmid.png", "2", "43", ""],
                        ["64", "krigyer.png", "2", "44", ""],
                        ["65", "kufman.png", "2", "45", ""],
                        ["66", "gornyiy_troll'.png", "2", "46", ""],
                        ["67", "snyezhnyiy_troll'.png", "2", "47", ""],
                        ["68", "bolotnyiy_troll'.png", "2", "48", ""],
                        ["69", "lyesnoiy_troll'.png", "2", "49", ""],
                        ["70", "udstiryets.png", "2", "50", ""],
                        ["71", "fiskiryets.png", "2", "51", ""],
                        ["72", "mont.png", "2", "52", ""],
                        ["73", "ogr.png", "2", "53", ""],
                        ["74", "al'v.png", "2", "54", ""],
                        ["75", "antropozavr.png", "2", "55", ""],
                        ["76", "elvin.png", "2", "56", ""],
                        ["77", "danu_nation.png", "2", "57", ""],
                        ["78", "alvraat_empire.png", "4", "1", ""],
                        ["79", "principality_saorsa.png", "4", "2", ""],
                        ["80", "kingdom_bergen.png", "4", "3", ""],
                        ["81", "fesgar_principality.png", "4", "4", ""],
                        ["82", "sverdensky_kaganate.png", "4", "5", ""],
                        ["83", "khanate_tavalin.png", "4", "6", ""],
                        ["84", "principality_sargib.png", "4", "7", ""],
                        ["85", "raj_bandu.png", "4", "8", ""],
                        ["86", "kingdom_norder.png", "4", "9", ""],
                        ["87", "alter_principality.png", "4", "10", ""],
                        ["88", "orliadar_confederation.png", "4", "11", ""],
                        ["89", "kingdom_udstir.png", "4", "12", ""],
                        ["90", "kingdom_vervirung.png", "4", "13", ""],
                        ["91", "destin_order.png", "4", "14", ""],
                        ["92", "free_city_liyset.png", "4", "15", ""],
                        ["93", "liscian_empire.png", "4", "16", ""],
                        ["94", "kingdom_valtir.png", "4", "17", ""],
                        ["95", "vassal_principality_gratis.png", "4", "18", ""],
                        ["96", "principality_rekta.png", "4", "19", ""],
                        ["97", "volar.png", "4", "20", ""],
                        ["98", "union_il_ladro.png", "4", "21", ""],
                        ["99", "merger_union.png", "4", "22", ""],
                        ["100", "government.png", "5", "2", ""],
                        ["101", "aristocracy.png", "5", "3", ""],
                        ["102", "clergy.png", "5", "4", ""],
                        ["103", "magicians.png", "5", "5", ""],
                        ["104", "military.png", "5", "6", ""],
                        ["105", "merchants.png", "5", "7", ""],
                        ["106", "criminality.png", "5", "8", ""],
                        ["107", "intelligentsia.png", "5", "9", ""],
                        ["108", "factionless.png", "5", "10", ""],
                        ["109", "logo.png", "6", "1", ""],
                        ["110", "about_project.png", "6", "2", ""],
                        ["111", "dark_icon.svg", "6", "3", ""],
                        ["112", "news_start.png", "7", "2", ""],
                        ["113", "news_start_authorization.png", "7", "3", ""],
                        ["114", "news_start_lending.png", "7", "4", ""],
                    ];

                    //Проход по коллекции ключей
                    foreach (var key in keys)
                    {
                        //Добавление сущности в бд при её отсутствии
                        if (!_filesContext.Files.Any(x => x.Id == long.Parse(key[0])))
                        {
                            //Получение сущностей
                            FileType fileType = await _filesContext.FilesTypes.FirstOrDefaultAsync(x => x.Id == long.Parse(key[2])) ?? throw new Exception(ErrorMessagesFiles.NotFoundFileType + key[2]);

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