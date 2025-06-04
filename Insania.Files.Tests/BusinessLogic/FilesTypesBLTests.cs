using Microsoft.Extensions.DependencyInjection;

using Insania.Shared.Models.Responses.Base;

using Insania.Files.Contracts.BusinessLogic;
using Insania.Files.Tests.Base;

namespace Insania.Files.Tests.BusinessLogic;

/// <summary>
/// Тесты сервиса работы с бизнес-логикой типов файлов
/// </summary>
[TestFixture]
public class FilesTypesBLTests : BaseTest
{
    #region Поля
    /// <summary>
    /// Сервис работы с бизнес-логикой типов файлов
    /// </summary>
    private IFilesTypesBL FilesTypesBL { get; set; }
    #endregion

    #region Общие методы
    /// <summary>
    /// Метод, вызываемый до тестов
    /// </summary>
    [SetUp]
    public void Setup()
    {
        //Получение зависимости
        FilesTypesBL = ServiceProvider.GetRequiredService<IFilesTypesBL>();
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
            BaseResponseList? result = await FilesTypesBL.GetList();

            //Проверка результата
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Items, Is.Not.Null);
                Assert.That(result.Items, Is.Not.Empty);
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