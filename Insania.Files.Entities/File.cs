using Insania.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Insania.Files.Entities;

/// <summary>
/// Модель сущности файла
/// </summary>
[Table("r_files")]
[Comment("Файлы")]
public class File : Reestr
{
    #region Конструкторы
    /// <summary>
    /// Простой конструктор модели сущности файла
    /// </summary>
    public File() : base()
    {
        Name = string.Empty;
        Extension = string.Empty;
        TypeEntity = new();
    }

    /// <summary>
    /// Конструктор модели сущности файла без id
    /// </summary>
    /// <param cref="string" name="username">Логин пользователя, выполняющего действие</param>
    /// <param cref="bool" name="isSystem">Признак системной записи</param>
    /// <param cref="string" name="name">Наименование</param>
    /// <param cref="FileType" name="type">Идентификатор типа</param>
    /// <param cref="DateTime?" name="dateDeleted">Дата удаления</param>
    public File(string username, bool isSystem, string name, FileType type, DateTime? dateDeleted = null) : base(username, isSystem, dateDeleted)
    {
        Name = name;
        TypeEntity = type;
        TypeId = type.Id;
        Extension = name[(name.LastIndexOf('.') + 1)..];
    }

    /// <summary>
    /// Конструктор модели сущности файла c id
    /// </summary>
    /// <param cref="long" name="id">Первичный ключ таблицы</param>
    /// <param cref="string" name="username">Логин пользователя, выполняющего действие</param>
    /// <param cref="bool" name="isSystem">Признак системной записи</param>
    /// <param cref="string" name="name">Наименование</param>
    /// <param cref="FileType" name="type">Идентификатор типа</param>
    /// <param cref="DateTime?" name="dateDeleted">Дата удаления</param>
    public File(long id, string username, bool isSystem, string name, FileType type, DateTime? dateDeleted = null) : base(id, username, isSystem, dateDeleted)
    {
        Name = name;
        TypeEntity = type;
        TypeId = type.Id;
        Extension = name[(name.LastIndexOf('.') + 1)..];
    }
    #endregion

    #region Поля
    /// <summary>
    /// Наименование 
    /// </summary>
    [Column("name")]
    [Comment("Наименование")]
    public string Name { get; private set; }

    /// <summary>
    /// Расширение
    /// </summary>
    [Column("extension")]
    [Comment("Расширение")]
    public string Extension { get; private set; }

    /// <summary>
    /// Идентификатор типа
    /// </summary>
    [Column("type_id")]
    [Comment("Идентификатор типа")]
    [ForeignKey(nameof(TypeEntity))]
    public long TypeId { get; private set; }
    #endregion

    #region Навигационные свойства
    /// <summary>
    /// Навигационное свойство типа
    /// </summary>
    public FileType TypeEntity { get; private set; }
    #endregion

    #region Методы
    /// <summary>
    /// Метод записи наименования
    /// </summary>
    /// <param cref="string" name="name">Наименование</param>
    public void SetName(string name)
    {
        Name = name;
        Extension = name[(name.LastIndexOf('.') + 1)..];
    }

    /// <summary>
    /// Метод записи типа
    /// </summary>
    /// <param cref="FileType" name="type">Идентификатор типа</param>
    public void SetType(FileType type)
    {
        TypeId = type.Id;
        TypeEntity = type;
    }
    #endregion
}