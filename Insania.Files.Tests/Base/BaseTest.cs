using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Insania.Shared.Contracts.DataAccess;
using Insania.Shared.Contracts.Services;
using Insania.Shared.Services;

using Insania.Files.BusinessLogic;
using Insania.Files.DataAccess;
using Insania.Files.Database.Contexts;
using Insania.Files.Models.Settings;
using Insania.Files.Models.Mapper;
using Insania.Files.Entities;

using File = System.IO.File;

namespace Insania.Files.Tests.Base;

/// <summary>
/// Базовый класс тестирования
/// </summary>
public abstract class BaseTest
{
    #region Конструкторы
    /// <summary>
    /// Простой конструктор базового класса тестирования
    /// </summary>
    public BaseTest()
    {
        //Создание коллекции сервисов
        IServiceCollection services = new ServiceCollection();

        //Создание коллекции ключей конфигурации
        Dictionary<string, string> configurationKeys = new()
        {
           {"LoggingOptions:FilePath", DetermineLogPath()},
           {"InitializationDataSettings:ScriptsPath", DetermineScriptsPath()},
           {"InitializationDataSettings:InitStructure", "false"},
           {"InitializationDataSettings:Tables:FilesTypes", "true"},
           {"InitializationDataSettings:Tables:Files", "true"},
           {"TokenSettings:Issuer", "Files.Test"},
           {"TokenSettings:Audience", "Files.Test"},
           {"TokenSettings:Key", "This key is generated for tests in the user zone"},
           {"TokenSettings:Expires", "7"},
        };

        //Создание экземпляра конфигурации в памяти
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationKeys!).Build();

        //Установка игнорирования типов даты и времени
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        //Внедрение зависимостей сервисов
        services.AddSingleton(_ => configuration); //конфигурация
        services.AddScoped<ITransliterationSL, TransliterationSL>(); //сервис транслитерации
        services.AddScoped<IInitializationDAO, InitializationDAO>(); //сервис инициализации данных в бд файлов
        services.AddFilesBL(); //сервисы работы с бизнес-логикой в зоне файлов

        //Добавление контекстов бд в коллекцию сервисов
        services.AddDbContext<FilesContext>(options => options.UseInMemoryDatabase(databaseName: "insania_files").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))); //бд файлов
        services.AddDbContext<LogsApiFilesContext>(options => options.UseInMemoryDatabase(databaseName: "insania_logs_api_files").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))); //бд логов сервиса файлов

        //Добавление параметров логирования
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(path: configuration["LoggingOptions:FilePath"]!, rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(Log.Logger, dispose: true));

        //Добавление параметров преобразования моделей
        services.AddAutoMapper(cfg => { cfg.AddProfile<FilesMappingProfile>(); });

        //Добавление параметров инициализации данных
        IConfigurationSection? initializationDataSettings = configuration.GetSection("InitializationDataSettings");
        services.Configure<InitializationDataSettings>(initializationDataSettings);

        //Создание поставщика сервисов
        ServiceProvider = services.BuildServiceProvider();

        //Обновление путей файлов
        UpdateFilePath().Wait();

        //Выполнение инициализации данных
        IInitializationDAO initialization = ServiceProvider.GetRequiredService<IInitializationDAO>();
        initialization.Initialize().Wait();
    }
    #endregion

    #region Поля
    /// <summary>
    /// Поставщик сервисов
    /// </summary>
    protected IServiceProvider ServiceProvider { get; set; }
    #endregion

    #region Методы
    /// <summary>
    /// Метод определения пути для логов
    /// </summary>
    /// <returns cref="string">Путь для сохранения логов</returns>
    private static string DetermineLogPath()
    {
        //Проверка запуска в докере
        bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" || File.Exists("/.dockerenv");

        //Возврат нужного пути
        if (isRunningInDocker) return "/logs/log.txt";
        else return "G:\\Program\\Insania\\Logs\\Files.Tests\\log.txt";
    }

    /// <summary>
    /// Метод определения пути для скриптов
    /// </summary>
    /// <returns cref="string">Путь к скриптам</returns>
    private static string DetermineScriptsPath()
    {
        //Проверка запуска в докере
        bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" || File.Exists("/.dockerenv");
        
        //Возврат нужного пути
        if (isRunningInDocker) return "/src/Insania.Files.Database/Scripts";
        else return "G:\\Program\\Insania\\Insania.Files\\Insania.Files.Database\\Scripts";
    }

    /// <summary>
    /// Метод обновления пути файлов
    /// </summary>
    private async Task UpdateFilePath()
    {
        //Проверка запуска в докере
        bool isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" || File.Exists("/.dockerenv");
        if (isRunningInDocker)
        {
            //Получение контекста бд
            FilesContext context = ServiceProvider.GetRequiredService<FilesContext>();

            //Получение типов файлов
            List<FileType> types = await context.FilesTypes.ToListAsync();

            //Обновление путей типов файлов
            foreach (FileType type in types)
            {
                type.SetPath("/files");
            }

            //Сохранение изменений
            await context.SaveChangesAsync();
        }
    }
    #endregion
}