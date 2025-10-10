using Microsoft.Extensions.DependencyInjection;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.Contracts.DataAccess;
using Insania.Files.Models.Responses;
using Insania.Files.Tests.Base;

using ErrorMessagesShared = Insania.Shared.Messages.ErrorMessages;

using ErrorMessagesFiles = Insania.Files.Messages.ErrorMessages;

namespace Insania.Files.Tests.BusinessLogic;

/// <summary>
/// Тесты сервиса работы с бизнес-логикой файлов
/// </summary>
[TestFixture]
public class FilesBLTests : BaseTest
{
    #region Поля
    /// <summary>
    /// Сервис работы с бизнес-логикой файлов
    /// </summary>
    private IFilesBL FilesBL { get; set; }

    private IFilesTypesDAO FilesTypesDAO { get; set; }
    #endregion

    #region Общие методы
    /// <summary>
    /// Метод, вызываемый до тестов
    /// </summary>
    [SetUp]
    public void Setup()
    {
        //Получение зависимости
        FilesBL = ServiceProvider.GetRequiredService<IFilesBL>();
        FilesTypesDAO = ServiceProvider.GetRequiredService<IFilesTypesDAO>();
    }

    /// <summary>
    /// Метод, вызываемый после тестов
    /// </summary>
    [TearDown]
    public void TearDown()
    {

    }
    #endregion

    #region Методы тестирования
    /// <summary>
    /// Тест метода получения файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор файла</param>
    [TestCase(null)]
    [TestCase(-1)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public async Task GetByIdTest(long? id)
    {
        try
        {
            var types = await FilesTypesDAO.GetList();
            throw new Exception(string.Join(',', types.Select(x => x.Path)));

            //Получение результата
            FileResponse? result = await FilesBL.GetById(id);

            //Проверка результата
            Assert.That(result, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Stream, Is.Not.Null);
            }
            Assert.That(result.Stream.Length, Is.Positive);
            switch (id)
            {
                case 1: Assert.That(result.ContentType, Is.EqualTo("image/png")); break;
                default: throw new Exception(ErrorMessagesShared.NotFoundTestCase);
            }
        }
        catch (Exception ex)
        {
            //Проверка исключения
            switch (id)
            {
                case null: Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.EmptyFile)); break;
                case -1: case 2: Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.NotFoundFile)); break;
                case 3: Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.IncorrectContentType)); break;
                case 4: Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.DeletedFileType)); break;
                case 5: Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.DeletedFile)); break;
                default: throw;
            }
        }
    }

    /// <summary>
    /// Тест метода получения списка файла по идентификатору сущности и идентификатору типа
    /// </summary>
    /// <param cref="long?" name="entityId">Идентификатор сущности</param>
    /// <param cref="long?" name="typeId">Идентификатор типа</param>
    [TestCase(null, null)]
    [TestCase(-1, null)]
    [TestCase(-1, 1)]
    [TestCase(1, -1)]
    [TestCase(1, 3)]
    [TestCase(1, 1)]
    [TestCase(2, 1)]
    public async Task GetListTest(long? entityId, long? typeId)
    {
        try
        {
            //Получение результата
            BaseResponseList? result = await FilesBL.GetList(entityId, typeId);

            //Проверка результата
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            switch (entityId, typeId)
            {
                case (-1, 1): case (2, 1): case (1, -1): case (1, 3): Assert.That(result.Items, Is.Empty); break;
                case (1, 1): Assert.That(result.Items, Is.Not.Empty); break;
                default: throw new Exception(ErrorMessagesShared.NotFoundTestCase);
            }
        }
        catch (Exception ex)
        {
            //Проверка исключения
            switch (entityId, typeId)
            {
                case (null, null): Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.EmptyEntity)); break;
                case (-1, null): Assert.That(ex.Message, Is.EqualTo(ErrorMessagesFiles.EmptyFileType)); break;
                default: throw;
            }
        }
    }
    #endregion
}