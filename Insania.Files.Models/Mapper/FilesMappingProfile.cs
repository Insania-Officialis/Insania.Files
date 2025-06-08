using AutoMapper;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Entities;

using FileEntity = Insania.Files.Entities.File;

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

        //Преобразование модели сущности файла в базовую модель элемента ответа списком
        CreateMap<FileEntity, BaseResponseListItem>();
    }
}