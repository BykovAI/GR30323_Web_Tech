using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Services.ProductService;
using GR30323_Web.Domain.Services.CategoryService;

namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class CarEditModel : PageModel
    {
        private readonly IProductService _productService; // Сервис для работы с автомобилями
        private readonly ICategoryService _categoryService; // Сервис для работы с категориями

        public CarEditModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public GR30323_Web.Domain.Entities.Car Car { get; set; } = default!; // Автомобиль для редактирования

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Получаем автомобиль через ProductService
            var carResponse = await _productService.GetProductByIdAsync(id.Value);
            if (carResponse == null || !carResponse.Success || carResponse.Data == null)
            {
                return NotFound();
            }

            Car = carResponse.Data;

            // Загружаем список категорий
            var categoryResponse = await _categoryService.GetCategoryListAsync();
            if (categoryResponse?.Data != null)
            {
                ViewData["CategoryId"] = new SelectList(categoryResponse.Data, "Id", "Name", Car.CategoryId);
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(Enumerable.Empty<GR30323_Web.Domain.Entities.Category>(), "Id", "Name");
            }

            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Повторная загрузка списка категорий
                var categoryResponse = await _categoryService.GetCategoryListAsync();
                if (categoryResponse?.Data != null)
                {
                    ViewData["CategoryId"] = new SelectList(categoryResponse.Data, "Id", "Name");
                }
                else
                {
                    ViewData["CategoryId"] = new SelectList(Enumerable.Empty<GR30323_Web.Domain.Entities.Category>(), "Id", "Name");
                }

                return Page(); // Возвращаем форму редактирования
            }

            // Обновляем автомобиль через ProductService
            var updateResponse = await _productService.UpdateProductAsync(Car);

            if (!updateResponse.Success)
            {
                // Если обновление не удалось, показываем ошибку
                ModelState.AddModelError(string.Empty, updateResponse.ErrorMessage ?? "Ошибка при обновлении автомобиля.");
                return Page(); // Возвращаем форму редактирования
            }

            // Редиректим на Index
            return RedirectToPage("./Index");
        }







    }
}
