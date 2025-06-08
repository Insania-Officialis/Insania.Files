using Insania.Files.Models.Responses;

namespace Insania.Files.Contracts.BusinessLogic;

/// <summary>
/// Интерфейс работы с бизнес-логикой файлов
/// </summary>
public interface IFilesBL
{
    /// <summary>
    /// Метод получения файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор файла</param>
    /// <returns cref="FileResponse?">Ответ файла</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<FileResponse?> GetById(long? id);
}