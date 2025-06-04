using Microsoft.Extensions.DependencyInjection;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Tests.Base;

using Insania.Files.Entities;

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
    /// Тест метода получения списка файлов
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
    #endregion
}