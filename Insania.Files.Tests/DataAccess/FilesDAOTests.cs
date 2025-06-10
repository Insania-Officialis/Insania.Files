using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Messages;
using Insania.Files.Tests.Base;

using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.Tests.DataAccess;

/// <summary>
/// Тесты сервиса работы с данными файлов
/// </summary>
[TestFixture]
public class FilesDAOTests : BaseTest
{
    #region Поля
    /// <summary>
    /// Сервис работы с данными файлов
    /// </summary>
    private IFilesDAO FilesDAO { get; set; }
    #endregion

    #region Общие методы
    /// <summary>
    /// Метод, вызываемый до тестов
    /// </summary>
    [SetUp]
    public void Setup()
    {
        //Получение зависимости
        FilesDAO = ServiceProvider.GetRequiredService<IFilesDAO>();
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
    /// Тест метода получения списка файлов
    /// </summary>
    [Test]
    public async Task GetListTest()
    {
        try
        {
            //Получение результата
            List<FileEntity>? result = await FilesDAO.GetList();

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
    /// Тест метода получения файла по идентификатору
    /// </summary>
    /// <param cref="long?" name="id">Идентификатор файла</param>
    [TestCase(null)]
    [TestCase(-1)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task GetByIdTest(long? id)
    {
        try
        {
            //Получение результата
            FileEntity? result = await FilesDAO.GetById(id);

            //Проверка результата
            switch (id)
            {
                case -1: Assert.That(result, Is.Null); break;
                case 1: case 2: Assert.That(result, Is.Not.Null); break;
                default: throw new Exception(ErrorMessages.NotFoundTestCase);
            }
        }
        catch (Exception ex)
        {
            //Проверка исключения
            switch (id)
            {
                case null: Assert.That(ex.Message, Is.EqualTo(ErrorMessages.EmptyFile)); break;
                default: throw;
            }
        }
    }

    /// <summary>
    /// Тест метода получения списка файлов по идентификатору сущности
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
            List<FileEntity> result = await FilesDAO.GetList(entityId ,typeId);

            //Проверка результата
            Assert.That(result, Is.Not.Null);
            switch (entityId, typeId)
            {
                case (-1, 1): case (2, 1): case (1, -1): case (1, 3): Assert.That(result, Is.Empty); break;
                case (1, 1): Assert.That(result, Is.Not.Empty); break;
                default: throw new Exception(ErrorMessages.NotFoundTestCase);
            }
        }
        catch (Exception ex)
        {
            //Проверка исключения
            switch (entityId, typeId)
            {
                case (null, null): Assert.That(ex.Message, Is.EqualTo(ErrorMessages.EmptyEntity)); break;
                case (-1, null): Assert.That(ex.Message, Is.EqualTo(ErrorMessages.EmptyFileType)); break;
                default: throw;
            }
        }
    }
    #endregion
}