using Microsoft.EntityFrameworkCore;

using Insania.Files.Entities;

namespace Insania.Files.Database.Contexts;

/// <summary>
/// Контекст бд логов сервиса файлов
/// </summary>
public class LogsApiFilesContext : DbContext
{
    #region Конструкторы
    /// <summary>
    /// Простой конструктор контекста бд логов сервиса файлов
    /// </summary>
    public LogsApiFilesContext() : base()
    {

    }

    /// <summary>
    /// Конструктор контекста бд логов сервиса файлов с опциями
    /// </summary>
    /// <param cref="DbContextOptions{LogsApiFilesContext}" name="options">Параметры</param>
    public LogsApiFilesContext(DbContextOptions<LogsApiFilesContext> options) : base(options)
    {

    }
    #endregion

    #region Поля
    /// <summary>
    /// Пользователи
    /// </summary>
    public virtual DbSet<LogApiFiles> Logs { get; set; }
    #endregion

    #region Методы
    /// <summary>
    /// Метод при создании моделей
    /// </summary>
    /// <param cref="ModelBuilder" name="modelBuilder">Конструктор моделей</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Установка схемы бд
        modelBuilder.HasDefaultSchema("insania_logs_api_files");
        
        //Добавление gin-индекса на поле с входными данными логов
        modelBuilder.Entity<LogApiFiles>().HasIndex(x => x.DataIn).HasMethod("gin");

        //Добавление gin-индекса на поле с выходными данными логов
        modelBuilder.Entity<LogApiFiles>().HasIndex(x => x.DataOut).HasMethod("gin");
    }
    #endregion
}