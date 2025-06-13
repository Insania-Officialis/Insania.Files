using Microsoft.AspNetCore.Mvc;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Contracts.BusinessLogic;

using ErrorMessages = Insania.Shared.Messages.ErrorMessages;

namespace Insania.Files.ApiRead.Controllers;

/// <summary>
/// Контроллер работы с типами файлов
/// </summary>
/// <param cref="ILogger" name="logger">Сервис логгирования</param>
/// <param cref="IFilesTypesBL" name="filesTypesService">Сервис работы с бизнес-логикой типов файлов</param>
[Route("files_types")]
public class FilesTypesController(ILogger<FilesTypesController> logger, IFilesTypesBL filesTypesService) : Controller
{
    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<FilesTypesController> _logger = logger;

    /// <summary>
    /// Сервис работы с бизнес-логикой типов файлов
    /// </summary>
    private readonly IFilesTypesBL _filesTypesService = filesTypesService;
    #endregion

    #region Методы
    /// <summary>
    /// Метод получения списка типов файлов
    /// </summary>
    /// <returns cref="OkResult">Список типов файлов</returns>
    /// <returns cref="BadRequestResult">Ошибка</returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetList()
    {
        try
        {
            //Получение результата проверки логина
            BaseResponse? result = await _filesTypesService.GetList();

            //Возврат ответа
            return Ok(result);
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text} {ex}", ErrorMessages.Error, ex);

            //Возврат ошибки
            return BadRequest(new BaseResponseError(ex.Message));
        }
    }
    #endregion
}