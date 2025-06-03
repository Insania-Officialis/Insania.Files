using Microsoft.EntityFrameworkCore;

using Insania.Files.Entities;

using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.Database.Contexts;

/// <summary>
/// Контекст бд файлов
/// </summary>
public class FilesContext : DbContext
{
    #region Конструкторы
    /// <summary>
    /// Простой конструктор контекста бд файлов
    /// </summary>
    public FilesContext() : base()
    {

    }

    /// <summary>
    /// Конструктор контекста бд файлов с опциями
    /// </summary>
    /// <param cref="DbContextOptions{FilesContext}" name="options">Параметры</param>
    public FilesContext(DbContextOptions<FilesContext> options) : base(options)
    {

    }
    #endregion

    #region Поля
    /// <summary>
    /// Типы файлов
    /// </summary>
    public virtual DbSet<FileType> FilesTypes { get; set; }

    /// <summary>
    /// Файлы
    /// </summary>
    public virtual DbSet<FileEntity> Files { get; set; }
    #endregion

    #region Методы
    /// <summary>
    /// Метод при создании моделей
    /// </summary>
    /// <param cref="ModelBuilder" name="modelBuilder">Конструктор моделей</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Установка схемы бд
        modelBuilder.HasDefaultSchema("insania_files");

        //Создание ограничения уникальности на псевдоним типа файлов
        modelBuilder.Entity<FileType>().HasAlternateKey(x => x.Alias);
    }
    #endregion
}