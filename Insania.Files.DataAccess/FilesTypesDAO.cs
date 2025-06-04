using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Database.Contexts;
using Insania.Files.Entities;
using Insania.Files.Messages;

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
            _logger.LogError("{text}: {error}", ErrorMessages.Error, ex.Message);

            //Проброс исключения
            throw;
        }

    }
    #endregion
}