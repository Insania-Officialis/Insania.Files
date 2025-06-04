using Insania.Shared.Models.Responses.Base;

namespace Insania.Files.Contracts.BusinessLogic;

/// <summary>
/// Интерфейс работы с бизнес-логикой типов файлов
/// </summary>
public interface IFilesTypesBL
{
    /// <summary>
    /// Метод получения списка типов файлов
    /// </summary>
    /// <returns cref="BaseResponseList">Стандартный ответ</returns>
    /// <exception cref="Exception">Исключение</exception>
    Task<BaseResponseList> GetList();
}