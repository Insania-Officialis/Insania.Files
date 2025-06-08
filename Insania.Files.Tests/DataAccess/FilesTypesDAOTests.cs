using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Tests.Base;

using Insania.Files.Entities;
using Insania.Files.Messages;

namespace Insania.Files.Tests.DataAccess;

/// <summary>
/// Тесты сервиса работы с данными файлов
/// </summary>
[TestFixture]
public class FilesTypesDAOTests : BaseTest
{
    #region Поля
    /// <summary>
    /// Сервис работы с данными файлов
    /// </summary>
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
    /// Тест метода получения списка типов файлов
    /// </summary>
    [Test]
    public async Task GetListTest()
    {
        try
        {
            //Получение результата
            List<FileType>? result = await FilesTypesDAO.GetList();

            //Проверка результата
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
        catch (Exception)
        {
            //Проброс исключения
            throw;
        }
    }

    /// <summary>
    /// Тест метода получения типа файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор типа файла</param>
    [TestCase(null)]
    [TestCase(-1)]
    [TestCase(1)]
    [TestCase(3)]
    public async Task GetByIdTest(long? id)
    {
        try
        {
            //Получение результата
            FileType? result = await FilesTypesDAO.GetById(id);

            //Проверка результата
            switch (id)
            {
                case -1: Assert.That(result, Is.Null); break;
                case 1: case 3: Assert.That(result, Is.Not.Null); break;
                default: throw new Exception(ErrorMessages.NotFoundTestCase);
            }
        }
        catch (Exception ex)
        {
            //Проверка исключения
            switch (id)
            {
                case null: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.EmptyFileType)); break;
                default: throw;
            }
        }
    }
    #endregion
}