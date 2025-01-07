using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GR30323_Web.Domain.Entities; // Пространство имен для Car
using GR30323_Web.Domain.Services.ProductService; // Пространство для ProductService
using GR30323_Web.Domain.Services.CategoryService; // Пространство для CategoryService
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class CarCreateModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context; // Контекст базы данных
        private readonly IProductService _productService; // Сервис для работы с продуктами (автомобилями)
        private readonly ICategoryService _categoryService; // Сервис для работы с категориями

        // Конструктор для инициализации контекста и сервисов
        public CarCreateModel(
            GR30323_Web.UI.Data.AppDbContext context,
            IProductService productService,
            ICategoryService categoryService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
        }

        // Модель для binding (связывания) данных
        [BindProperty]
        public GR30323_Web.Domain.Entities.Car Car { get; set; } = default!;

        // Метод для загрузки данных при запросе страницы (GET)
        public async Task<IActionResult> OnGetAsync()
        {
            // Получение списка категорий через CategoryService
            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (categoryResponse?.Data != null)
            {
                ViewData["CategoryId"] = new SelectList(categoryResponse.Data, "Id", "Name");
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(Enumerable.Empty<GR30323_Web.Domain.Entities.Category>(), "Id", "Name");
            }

            return Page();
        }






        // Метод, который вызывается при отправке формы (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            // Проверка на валидность модели
            if (!ModelState.IsValid)
            {
                // Повторная загрузка категорий при ошибке
                var categoryResponse = await _categoryService.GetCategoryListAsync();
                ViewData["CategoryId"] = categoryResponse?.Data != null
                    ? new SelectList(categoryResponse.Data, "Id", "Name")
                    : new SelectList(Enumerable.Empty<GR30323_Web.Domain.Entities.Category>(), "Id", "Name");

                return Page();
            }

            // Сохранение автомобиля через ProductService
            var createResponse = await _productService.CreateProductAsync(Car);
            if (!createResponse.Success)
            {
                ModelState.AddModelError(string.Empty, createResponse.ErrorMessage ?? "Ошибка при создании автомобиля.");
                return Page();
            }

            // Перенаправление на страницу с индексом после успешного добавления
            return RedirectToPage("./Index");
        }


    }
}
