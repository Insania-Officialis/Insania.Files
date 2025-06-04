using Microsoft.Extensions.DependencyInjection;

using Insania.Shared.Contracts.DataAccess;

using Insania.Files.Contracts.DataAccess;
using Insania.Files.Entities;
using Insania.Files.Tests.Base;

using FileEntity = Insania.Files.Entities.File;

namespace Insania.Files.Tests.DataAccess;

/// <summary>
/// Тесты сервиса инициализации данных в бд файлов
/// </summary>
[TestFixture]
public class InitializationDAOTests : BaseTest
{
    #region Поля
    /// <summary>
    /// Сервис инициализации данных в бд файлов
    /// </summary>
    private IInitializationDAO InitializationDAO { get; set; }

    /// <summary>
    /// Сервис работы с данными типов файлов
    /// </summary>
    private IFilesTypesDAO FilesTypesDAO { get; set; }

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
        InitializationDAO = ServiceProvider.GetRequiredService<IInitializationDAO>();
        FilesTypesDAO = ServiceProvider.GetRequiredService<IFilesTypesDAO>();
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
    /// Тест метода инициализации данных
    /// </summary>
    [Test]
    public async Task InitializeTest()
    {
        try
        {
            //Выполнение метода
            await InitializationDAO.Initialize();

            //Получение сущностей
            List<FileType> filesTypes = await FilesTypesDAO.GetList();
            List<FileEntity> files = await FilesDAO.GetList();

            //Проверка результата
            Assert.Multiple(() =>
            {
                Assert.That(filesTypes, Is.Not.Empty);
                Assert.That(files, Is.Not.Empty);
            });
        }
        catch (Exception)
        {
            //Проброс исключения
            throw;
        }
    }
    #endregion
}