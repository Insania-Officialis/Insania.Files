using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Database.Contexts;
using Insania.Files.Entities;

using ErrorMessagesShared = Insania.Shared.Messages.ErrorMessages;

using ErrorMessagesFiles = Insania.Files.Messages.ErrorMessages;
using InformationMessages = Insania.Files.Messages.InformationMessages;

namespace Insania.Files.DataAccess;

/// <summary>
/// Сервис работы с данными типов файлов
/// </summary>
/// <param cref="ILogger{FilesTypesDAO}" name="logger">Сервис логгирования</param>
/// <param cref="FilesContext" name="context">Контекст базы данных файлов</param>
public class FilesTypesDAO(ILogger<FilesTypesDAO> logger, FilesContext context) : IFilesTypesDAO
{
    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<FilesTypesDAO> _logger = logger;

    /// <summary>
    /// Контекст базы данных файлов
    /// </summary>
    private readonly FilesContext _context = context;
    #endregion

    #region Методы
    /// <summary>
    /// Метод получения списка типов файлов
    /// </summary>
    /// <returns cref="List{FileType}">Список типов файлов</returns>
    /// <exception cref="Exception">Исключение</exception>
    public async Task<List<FileType>> GetList()
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredGetListFilesTypesMethod);

            //Получение данных из бд
            List<FileType> data = await _context.FilesTypes.Where(x => x.DateDeleted == null).ToListAsync();

            //Возврат результата
            return data;
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text}: {error}", ErrorMessagesShared.Error, ex.Message);

            //Проброс исключения
            throw;
        }

    }

    /// <summary>
    /// Метод получения типа файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор типа файла</param>
    /// <returns cref="FileType?">Тип файла</returns>
    /// <exception cref="Exception">Исключение</exception>
    public async Task<FileType?> GetById(long? id)
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredGetByIdFileTypeMethod);

            //Проверки
            if (id == null) throw new Exception(ErrorMessagesFiles.EmptyFileType);

            //Получение данных из бд
            FileType? data = await _context.FilesTypes.FirstOrDefaultAsync(x => x.Id == id);

            //Возврат результата
            return data;
        }
        catch (Exception ex)
        {
            //Логгирование
            _logger.LogError("{text}: {error}", ErrorMessagesShared.Error, ex.Message);

            //Проброс исключения
            throw;
        }
    }
    #endregion
}