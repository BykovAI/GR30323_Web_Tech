using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR30323_Web.API.Controllers;
using GR30323_Web.API.Data;
using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace GR30323_Web.Tests
{
    public class CarsApiControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private readonly IWebHostEnvironment _environment;

        public CarsApiControllerTests()
        {
            // Создаем мок IWebHostEnvironment
            _environment = Substitute.For<IWebHostEnvironment>();

            // Создаем in-memory SQLite базу данных
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // Настраиваем DbContextOptions для использования этой базы
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Создаем схему и добавляем начальные данные
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();

            // Добавляем тестовые данные
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "SUVs", NormalizedName = "suvs" },
                new Category { Id = 2, Name = "Sedans", NormalizedName = "sedans" }
            };

            var cars = new List<Car>
            {
                new Car { Id = 1, Name = "Car1", Description = "Description1", Price = 10000, CategoryId = 1 },
                new Car { Id = 2, Name = "Car2", Description = "Description2", Price = 20000, CategoryId = 1 },
                new Car { Id = 3, Name = "Car3", Description = "Description3", Price = 30000, CategoryId = 2 },
                new Car { Id = 4, Name = "Car4", Description = "Description4", Price = 40000, CategoryId = 2 },
                new Car { Id = 5, Name = "Car5", Description = "Description5", Price = 50000, CategoryId = 2 }
            };

            context.Categories.AddRange(categories);
            context.Cars.AddRange(cars);
            context.SaveChanges();
        }

        public void Dispose() => _connection.Dispose();

        private AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        // Тест: Проверка фильтрации по категории
        [Fact]
        public async Task ControllerFiltersCategory()
        {
            using var context = CreateContext();
            var controller = new CarsController(context);

            // Arrange: выбираем категорию для фильтрации
            var category = context.Categories.First();

            // Act: отправляем запрос на получение автомобилей для этой категории
            var response = await controller.GetCars(category.NormalizedName);
            var responseData = response.Value;
            var carsList = responseData.Data.Items;

            // Assert: все автомобили принадлежат выбранной категории
            Assert.NotNull(carsList);
            Assert.All(carsList, car => Assert.Equal(category.Id, car.CategoryId));
        }

        // Тест: Проверка количества страниц
        [Theory]
        [InlineData(2, 3)] // 5 машин с размером страницы 2 -> 3 страницы
        [InlineData(3, 2)] // 5 машин с размером страницы 3 -> 2 страницы
        public async Task ControllerReturnsCorrectPagesCount(int pageSize, int expectedPages)
        {
            using var context = CreateContext();
            var controller = new CarsController(context);

            // Act: отправляем запрос на получение данных с указанным размером страницы
            var response = await controller.GetCars(null, 1, pageSize);
            var responseData = response.Value;
            var totalPages = responseData.Data.TotalPages;

            // Assert: общее количество страниц совпадает с ожидаемым
            Assert.Equal(expectedPages, totalPages);
        }

        // Тест: Проверка возврата корректной страницы
        [Fact]
        public async Task ControllerReturnsCorrectPage()
        {
            using var context = CreateContext();
            var controller = new CarsController(context);

            // Arrange: ожидаем 2 объекта на второй странице
            int expectedItemsOnPage = 2;
            int pageNo = 2;
            Car firstCarOnPage = context.Cars.ToArray()[3]; // 4-й объект (индекс 3)

            // Act: запрашиваем вторую страницу с размером страницы 3
            var response = await controller.GetCars(null, pageNo, 3);
            var responseData = response.Value;
            var carsList = responseData.Data.Items;

            // Assert: проверяем данные страницы
            Assert.Equal(expectedItemsOnPage, carsList.Count); // Количество объектов на странице
            Assert.Equal(pageNo, responseData.Data.CurrentPage); // Номер страницы
            Assert.Equal(firstCarOnPage.Id, carsList[0].Id); // Первый объект на странице совпадает
        }
    }
}
