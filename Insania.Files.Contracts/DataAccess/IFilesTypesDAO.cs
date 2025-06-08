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

    /// <summary>
    /// Метод получения типа файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор типа файла</param>
    /// <returns cref="FileType?">Тип файла</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<FileType?> GetById(long? id);
}