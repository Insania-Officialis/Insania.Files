using Microsoft.AspNetCore.Mvc;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.Models.Responses;

using ErrorMessagesShared = Insania.Shared.Messages.ErrorMessages;

using ErrorMessagesFiles = Insania.Files.Messages.ErrorMessages;

namespace Insania.Files.ApiRead.Controllers;

/// <summary>
/// Контроллер работы с файлов
/// </summary>
/// <param cref="ILogger" name="logger">Сервис логгирования</param>
/// <param cref="IFilesBL" name="filesService">Сервис работы с бизнес-логикой файлов</param>
[Route("files")]
public class FilesController(ILogger<FilesController> logger, IFilesBL filesService) : Controller
{
    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<FilesController> _logger = logger;

    /// <summary>
    /// Сервис работы с бизнес-логикой файлов
    /// </summary>
    private readonly IFilesBL _filesService = filesService;
    #endregion

    #region Методы
    /// <summary>
    /// Метод получения файла по идентификатору
    /// </summary>
    /// <param cref="long" name="id">Идентификатор файла</param>
    /// <returns cref="OkResult">Список типов файлов</returns>
    /// <returns cref="BadRequestResult">Ошибка</returns>
    [HttpGet]
    [Route("by_id")]
    public async Task<IActionResult> GetById([FromQuery] long? id)
    {
        try
        {
            //Получение результата проверки логина
            FileResponse? result = await _filesService.GetById(id) ?? throw new Exception(ErrorMessagesFiles.NotFoundFile);

            //Проверки результата
            if (result.Stream == null) throw new Exception(ErrorMessagesFiles.NotFoundFile);
            if (result.ContentType == null) throw new Exception(ErrorMessagesFiles.IncorrectContentType);

            //Возврат ответа
            return File(result.Stream, result.ContentType);
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text} {ex}", ErrorMessagesShared.Error, ex);

            //Возврат ошибки
            return BadRequest(new BaseResponseError(ex.Message));
        }
    }

    /// <summary>
    /// Метод получения списка файлов по идентификатору сущности и идентификатору типа
    /// </summary>
    /// <param cref="long" name="entity_id">Идентификатор сущности</param>
    /// <param cref="long" name="type_id">Идентификатор типа</param>
    /// <returns cref="OkResult">Список файлов</returns>
    /// <returns cref="BadRequestResult">Ошибка</returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetList([FromQuery] long? entity_id, [FromQuery] long? type_id)
    {
        try
        {
            //Получение результата проверки логина
            BaseResponseList? result = await _filesService.GetList(entity_id, type_id);

            //Возврат ответа
            return Ok(result);
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text} {ex}", ErrorMessagesShared.Error, ex);

            //Возврат ошибки
            return BadRequest(new BaseResponseError(ex.Message));
        }
    }
    #endregion
}