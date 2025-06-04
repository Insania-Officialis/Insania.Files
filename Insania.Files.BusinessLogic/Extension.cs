using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.DataAccess;

namespace Insania.Files.BusinessLogic;

/// <summary>
/// Расширение для внедрения зависимостей сервисов работы с бизнес-логикой в зоне файлов
/// </summary>
public static class Extension
{
    /// <summary>
    /// Метод внедрения зависимостей сервисов работы с бизнес-логикой в зоне файлов
    /// </summary>
    /// <param cref="IServiceCollection" name="services">Исходная коллекция сервисов</param>
    /// <returns cref="IServiceCollection">Модифицированная коллекция сервисов</returns>
    public static IServiceCollection AddFilesBL(this IServiceCollection services) =>
        services
            .AddFilesDAO() //сервисы работы с данными в зоне файлов
            .AddScoped<IFilesTypesBL, FilesTypesBL>() //сервис работы с бизнес-логикой типов файлов
        ;
}