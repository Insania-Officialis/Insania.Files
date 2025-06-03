using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Insania.Shared.Contracts.Services;
using Insania.Shared.Entities;

namespace Insania.Files.Entities;

/// <summary>
/// Модель сущности типа файла
/// </summary>
[Table("d_files_types")]
[Comment("Типы файлов")]
public class FileType : Compendium
{
    #region Конструкторы
    /// <summary>
    /// Простой конструктор модели сущности типа файла
    /// </summary>
    public FileType() : base()
    {
        Path = string.Empty;
    }

    /// <summary>
    /// Конструктор модели сущности типа файла без идентификатора
    /// </summary>
    /// <param cref="ITransliterationSL" name="transliteration">Сервис транслитерации</param>
    /// <param cref="string" name="username">Логин пользователя, выполняющего действие</param>
    /// <param cref="string" name="name">Наименование</param>
    /// <param cref="string" name="path">Путь</param>
    /// <param cref="DateTime?" name="dateDeleted">Дата удаления</param>
    public FileType(ITransliterationSL transliteration, string username, string name, string path, DateTime? dateDeleted = null) : base(transliteration, username, name, dateDeleted)
    {
        Path = path;
    }

    /// <summary>
    /// Конструктор модели сущности типа файла с идентификатором
    /// </summary>
    /// <param cref="ITransliterationSL" name="transliteration">Сервис транслитерации</param>
    /// <param cref="long?" name="id">Идентификатор пользователя</param>
    /// <param cref="string" name="username">Логин пользователя, выполняющего действие</param>
    /// <param cref="string" name="name">Наименование</param>
    /// <param cref="string" name="path">Путь</param>
    /// <param cref="DateTime?" name="dateDeleted">Дата удаления</param>
    public FileType(ITransliterationSL transliteration, long id, string username, string name, string path, DateTime? dateDeleted = null) : base(transliteration, id, username, name, dateDeleted)
    {
        Path = path;
    }
    #endregion

    #region Поля
    /// <summary>
    ///	Путь
    /// </summary>
    [Column("path")]
    [Comment("Путь")]
    public string Path { get; private set; }
    #endregion

    #region Методы
    /// <summary>
    /// Метод записи пути
    /// </summary>
    /// <param cref="string" name="path">Путь</param>
    public void SetPath(string path)
    {
        Path = path;
    }
    #endregion
}