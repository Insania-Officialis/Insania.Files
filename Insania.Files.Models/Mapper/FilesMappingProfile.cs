using AutoMapper;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Entities;

namespace Insania.Files.Models.Mapper;

/// <summary>
/// Сервис преобразования моделей
/// </summary>
public class FilesMappingProfile : Profile
{
    /// <summary>
    /// Конструктор сервиса преобразования моделей
    /// </summary>
    public FilesMappingProfile()
    {
        //Преобразование модели сущности типа файла в базовую модель элемента ответа списком
        CreateMap<FileType, BaseResponseListItem>();
    }
}