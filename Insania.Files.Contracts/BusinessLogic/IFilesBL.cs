using Insania.Shared.Models.Responses.Base;

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

    /// <summary>
    /// Метод получения списка файлов по идентификатору сущности и идентификатору типа
    /// </summary>
    /// <param cref="long?" name="entityId">Идентификатор сущности</param>
    /// <param cref="long?" name="typeId">Идентификатор типа</param>
    /// <returns cref="BaseResponseList">Стандартный ответ списком</returns>
    /// <remarks>Список идентификатор файлов</remarks>
    /// <exception cref="Exception">Исключение</exception>
    Task<BaseResponseList?> GetList(long? entityId, long? typeId);
}