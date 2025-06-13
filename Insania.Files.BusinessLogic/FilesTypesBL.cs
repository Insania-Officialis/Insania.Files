using Microsoft.Extensions.Logging;

using AutoMapper;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.Contracts.DataAccess;
using Insania.Files.Entities;
using Insania.Files.Messages;

using ErrorMessages = Insania.Shared.Messages.ErrorMessages;

namespace Insania.Files.BusinessLogic;

/// <summary>
/// Сервис работы с бизнес-логикой типов файлов
/// </summary>
/// <param cref="ILogger{FilesTypesBL}" name="logger">Сервис логгирования</param>
/// <param cref="IMapper" name="mapper">Сервис преобразования моделей</param>
/// <param cref="IFilesTypesDAO" name="filesTypesDAO">Сервис работы с данными типов файлов</param>
public class FilesTypesBL(ILogger<FilesTypesBL> logger, IMapper mapper, IFilesTypesDAO filesTypesDAO) : IFilesTypesBL
{
    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<FilesTypesBL> _logger = logger;

    /// <summary>
    /// Сервис преобразования моделей
    /// </summary>
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Сервис работы с данными типов файлов
    /// </summary>
    private readonly IFilesTypesDAO _filesTypesDAO = filesTypesDAO;
    #endregion

    #region Методы
    /// <summary>
    /// Метод получения списка типов файлов
    /// </summary>
    /// <returns cref="BaseResponseList">Стандартный ответ</returns>
    /// <exception cref="Exception">Исключение</exception>
    public async Task<BaseResponseList> GetList()
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredGetListFilesTypesMethod);

            //Получение данных
            List<FileType>? data = await _filesTypesDAO.GetList();

            //Формирование ответа
            BaseResponseList? response = null;
            if (data == null) response = new(false, null);
            else response = new(true, data?.Select(_mapper.Map<BaseResponseListItem>).ToList());

            //Возврат ответа
            return response;
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text}: {error}", ErrorMessages.Error, ex.Message);

            //Проброс исключения
            throw;
        }
    }
    #endregion
}