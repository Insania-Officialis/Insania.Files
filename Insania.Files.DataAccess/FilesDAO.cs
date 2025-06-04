using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Database.Contexts;
using Insania.Files.Messages;

using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.DataAccess;

/// <summary>
/// Сервис работы с данными файлов
/// </summary>
/// <param cref="ILogger{FilesDAO}" name="logger">Сервис логгирования</param>
/// <param cref="FilesContext" name="context">Контекст базы данных файлов</param>
public class FilesDAO(ILogger<FilesDAO> logger, FilesContext context) : IFilesDAO
{
    #region Зависимости
    /// <summary>
    /// Сервис логгирования
    /// </summary>
    private readonly ILogger<FilesDAO> _logger = logger;

    /// <summary>
    /// Контекст базы данных файлов
    /// </summary>
    private readonly FilesContext _context = context;
    #endregion

    #region Методы
    /// <summary>
    /// Метод получения списка файлов
    /// </summary>
    /// <returns cref="List{FileEntity}">Список файлов</returns>
    /// <exception cref="Exception">Исключение</exception>
    public async Task<List<FileEntity>> GetList()
    {
        try
        {
            //Логгирование
            _logger.LogInformation(InformationMessages.EnteredGetListFilesMethod);

            //Получение данных из бд
            List<FileEntity> data = await _context.Files.Where(x => x.DateDeleted == null).ToListAsync();

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