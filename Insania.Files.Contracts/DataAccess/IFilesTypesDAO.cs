using Insania.Files.Entities;

namespace Insania.Files.Contracts.DataAccess;

/// <summary>
/// Интерфейс работы с данным типов файлов
/// </summary>
public interface IFilesTypesDAO
{
    /// <summary>
    /// Метод получения списка типов файлов
    /// </summary>
    /// <returns cref="List{FileType}">Список типов файлов</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<List<FileType>> GetList();
}