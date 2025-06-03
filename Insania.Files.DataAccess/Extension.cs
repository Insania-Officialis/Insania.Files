using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.DataAccess;

namespace Insania.Files.DataAccess;

/// <summary>
/// Расширение для внедрения зависимостей сервисов работы с данными в зоне файлов
/// </summary>
public static class Extension
{
    /// <summary>
    /// Метод внедрения зависимостей сервисов работы с данными в зоне файлов
    /// </summary>
    /// <param cref="IServiceCollection" name="services">Исходная коллекция сервисов</param>
    /// <returns cref="IServiceCollection">Модифицированная коллекция сервисов</returns>
    public static IServiceCollection AddFilesDAO(this IServiceCollection services) =>
        services
        ;
}