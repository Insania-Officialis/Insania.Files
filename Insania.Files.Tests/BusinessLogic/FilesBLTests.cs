using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.Messages;
using Insania.Files.Models.Responses;
using Insania.Files.Tests.Base;

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
            //Получение результата
            FileResponse? result = await FilesBL.GetById(id);

            //Проверка результата
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Stream, Is.Not.Null);
            });
            Assert.That(result.Stream.Length, Is.Positive);
            switch (id)
            {
                case 1: Assert.That(result.ContentType, Is.EqualTo("image/png")); break;
                default: throw new Exception(ErrorMessages.NotFoundTestCase);
            }
        }
        catch (Exception ex)
        {
            //Проверка исключения
            switch (id)
            {
                case null: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.EmptyFile)); break;
                case -1: case 2: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.NotFoundFile)); break;
                case 3: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.IncorrectContentType)); break;
                case 4: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.DeletedFileType)); break;
                case 5: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.DeletedFile)); break;
                default: throw;
            }
        }
    }
    #endregion
}