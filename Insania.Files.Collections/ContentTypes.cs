namespace Insania.Files.Collections;

/// <summary>
/// Класс типов контента
/// </summary>
public static class ContentTypes
{
    /// <summary>
    /// Значения типов контента
    /// </summary>
    public static readonly Dictionary<string, string> Values = new()
    {
        { "gif", "image/gif" },
        { "jpeg", "image/jpeg" },
        { "jpg", "image/jpeg" },
        { "png", "image/png" },
        { "tiff", "image/tiff" },
        { "webp", "image/webp" }
    };
}