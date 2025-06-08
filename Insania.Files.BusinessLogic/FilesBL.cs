using Microsoft.Extensions.Logging;

using AutoMapper;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Collections;
using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.Contracts.DataAccess;
using Insania.Files.Entities;
using Insania.Files.Messages;
using Insania.Files.Models.Responses;

using File = System.IO.File;

using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.BusinessLogic;

/// <summary>
/// Сервис работы с бизнес-логикой файлов
/// </summary>
/// <param cref="ILogger{FilesBL}" name="logger">Сервис логгирования</param>
/// <param cref="IMapper" name="mapper">Сервис преобразования моделей</param>
/// <param cref="IFilesDAO" name="filesDAO">Сервис работы с данными файлов</param>
/// <param cref="IFilesTypesDAO" name="filesTypesDAO">Сервис работы с данными типов файлов</param>
public class FilesBL(ILogger<FilesBL> logger, IMapper mapper, IFilesDAO filesDAO, IFilesTypesDAO filesTypesDAO) : IFilesBL
{
    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<FilesBL> _logger = logger;

    /// <summary>
    /// Сервис преобразования моделей
    /// </summary>
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Сервис работы с данными файлов
    /// </summary>
    private readonly IFilesDAO _filesDAO = filesDAO;

    /// <summary>
    /// Сервис работы с данными типов файлов
    /// </summary>
    private readonly IFilesTypesDAO _filesTypesDAO = filesTypesDAO;
    #endregion

    #region Методы
    /// <summary>
    /// Метод получения файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор файла</param>
    /// <returns cref="FileResponse?">Ответ файла</returns>
    /// <exception cref="Exception">Исключение</exception>
    public async Task<FileResponse?> GetById(long? id)
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredGetByIdFileMethod);

            //Проверки
            if (id == null) throw new Exception(ErrorMessages.EmptyFile);

            //Получение данных
            FileEntity? data = await _filesDAO.GetById(id) ?? throw new Exception(ErrorMessages.NotFoundFile);

            //Проверки данных
            if (data.DateDeleted != null) throw new Exception(ErrorMessages.DeletedFile);

            //Получение типа файла
            FileType type = await _filesTypesDAO.GetById(data.TypeId) ?? throw new Exception(ErrorMessages.EmptyFileType);

            //Проверки данных
            if (type.DateDeleted != null) throw new Exception(ErrorMessages.DeletedFileType);

            //Определение типа контента
            ContentTypes.Values.TryGetValue(data.Extension, out string? contentType);

            //Проверки данных
            if (string.IsNullOrWhiteSpace(contentType)) throw new Exception(ErrorMessages.IncorrectContentType);

            //Формирование пути к файлу
            string filePath = Path.Combine(type.Path, type.Alias, data.EntityId.ToString(), data.Name);

            //Проверка наличия файла
            if (!File.Exists(filePath)) throw new Exception(ErrorMessages.NotFoundFile);

            //Формирование потока файла
            FileStream stream = new(filePath, FileMode.Open);

            //Формирование ответа
            FileResponse response = new(true, stream, contentType);

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

    /// <summary>
    /// Метод получения списка файлов по идентификатору сущности
    /// </summary>
    /// <param cref="long?" name="entityId">Идентификатор сущности</param>
    /// <returns cref="BaseResponseList">Стандартный ответ списком</returns>
    /// <remarks>Список идентификатор файлов</remarks>
    /// <exception cref="Exception">Исключение</exception>
    public async Task<BaseResponseList?> GetList(long? entityId)
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredGetListFilesMethod);

            //Проверки
            if (entityId == null) throw new Exception(ErrorMessages.EmptyEntity);

            //Получение данных
            List<FileEntity> data = await _filesDAO.GetList(entityId);

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