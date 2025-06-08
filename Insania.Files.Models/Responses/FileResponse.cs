using Insania.Shared.Models.Responses.Base;

namespace Insania.Files.Models.Responses;

/// <summary>
/// Модель ответа файла
/// </summary>
/// <param cref="bool" name="success">Признак успешности</param>
/// <param cref="FileStream?" name="stream">Поток файла</param>
/// <param cref="string?" name="contentType">Тип контента</param>
public class FileResponse(bool success, FileStream? stream = null, string? contentType = null) : BaseResponse(success)
{
    #region Поля
    /// <summary>
    /// Поток файла
    /// </summary>
    public FileStream? Stream { get; set; } = stream;

    /// <summary>
    /// Тип контента
    /// </summary>
    public string? ContentType { get; set; } = contentType;
    #endregion
}