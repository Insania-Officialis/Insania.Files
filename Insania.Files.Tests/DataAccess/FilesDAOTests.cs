using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.DataAccess;
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
    #endregion
}