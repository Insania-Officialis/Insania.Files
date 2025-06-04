using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.Contracts.DataAccess;

/// <summary>
/// Интерфейс работы с данным файлов
/// </summary>
public interface IFilesDAO
{
    /// <summary>
    /// Метод получения списка файлов
    /// </summary>
    /// <returns cref="List{FileEntity}">Список файлов</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<List<FileEntity>> GetList();
}