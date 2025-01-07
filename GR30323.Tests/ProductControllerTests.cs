using System.Collections.Generic;
using System.Threading.Tasks;
using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Models;
using GR30323_Web.Domain.Services.CategoryService;
using GR30323_Web.Domain.Services.ProductService;
using GR30323_Web.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace GR30323_Web.Tests
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly ICategoryService _categoryServiceMock;
        private readonly IProductService _productServiceMock;

        public ProductControllerTests()
        {
            // Создаем моки для ICategoryService и IProductService
            _categoryServiceMock = Substitute.For<ICategoryService>();
            _productServiceMock = Substitute.For<IProductService>();

            // Создаем экземпляр контроллера с использованием моков
            _controller = new ProductController(_productServiceMock, _categoryServiceMock);
        }

        [Fact]
        public async Task Index_ShouldSetCategoriesInViewData()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1", NormalizedName = "category1" },
                new Category { Id = 2, Name = "Category2", NormalizedName = "category2" }
            };

            _categoryServiceMock.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>> { Success = true, Data = categories }));

            _productServiceMock.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ListModel<Car>>
                {
                    Success = true,
                    Data = new ListModel<Car>
                    {
                        Items = new List<Car>(),
                        CurrentPage = 1,
                        TotalPages = 1
                    }
                }));

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result); // Проверяем, что вернулся ViewResult
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.NotNull(viewResult.ViewData["categories"]);
            Assert.Equal(categories, viewResult.ViewData["categories"]);
        }

        [Fact]
        public async Task Index_ShouldSetCurrentCategoryInViewData()
        {
            // Arrange
            var categories = new List<Category>();
            _categoryServiceMock.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>> { Success = true, Data = categories }));

            _productServiceMock.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ListModel<Car>>
                {
                    Success = true,
                    Data = new ListModel<Car>
                    {
                        Items = new List<Car>(),
                        CurrentPage = 1,
                        TotalPages = 1
                    }
                }));

            // Act
            var result = await _controller.Index("category1");

            // Assert
            Assert.IsType<ViewResult>(result); // Проверяем, что вернулся ViewResult
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal("category1", viewResult.ViewData["currentCategory"]);
        }

        [Fact]
        public async Task Index_ShouldReturnNotFoundIfCategoryServiceFails()
        {
            // Arrange
            _categoryServiceMock.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>> { Success = false, ErrorMessage = "Error loading categories" }));

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result); // Проверяем, что вернулся NotFoundObjectResult
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal("Error loading categories", notFoundResult.Value); // Проверяем сообщение об ошибке
        }

        [Fact]
        public async Task Index_ShouldReturnPaginatedCars()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Id = 1, Name = "Tesla Model S", Price = 79999.99m },
                new Car { Id = 2, Name = "Toyota Corolla", Price = 20999.99m }
            };

            var paginatedCars = new ListModel<Car>
            {
                Items = cars,
                CurrentPage = 1,
                TotalPages = 1
            };

            _categoryServiceMock.GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>> { Success = true, Data = new List<Category>() }));

            _productServiceMock.GetProductListAsync(Arg.Any<string>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ListModel<Car>> { Success = true, Data = paginatedCars }));

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result); // Проверяем, что вернулся ViewResult
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            var model = viewResult.Model as ListModel<Car>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Items.Count);
            Assert.Equal("Tesla Model S", model.Items[0].Name);
            Assert.Equal("Toyota Corolla", model.Items[1].Name);
        }
    }
}
