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

    /// <summary>
    /// Метод получения файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор файла</param>
    /// <returns cref="FileEntity?">Файл</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<FileEntity?> GetById(long? id);

    /// <summary>
    /// Метод получения списка файлов по идентификатору сущности и идентификатору типа
    /// </summary>
    /// <param cref="long?" name="entityId">Идентификатор сущности</param>
    /// <param cref="long?" name="typeId">Идентификатор типа</param>
    /// <returns cref="List{FileEntity}">Список файлов</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<List<FileEntity>> GetList(long? entityId, long? typeId);
}