using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GR30323_Web.Domain.Entities; // Пространство имен для Car
using GR30323_Web.Domain.Services.ProductService; // Пространство имен для ProductService

namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class CarDeleteModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CarDeleteModel(IProductService productService, GR30323_Web.UI.Data.AppDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        // Используем полное имя типа для Car
        [BindProperty]
        public GR30323_Web.Domain.Entities.Car Car { get; set; } = default!;

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Удаляем автомобиль через ProductService
            var deleteResponse = await _productService.DeleteCarAsync(id.Value);
            if (!deleteResponse.Success)
            {
                ModelState.AddModelError(string.Empty, deleteResponse.ErrorMessage ?? "Ошибка при удалении автомобиля.");
                return Page(); // Возвращаемся на страницу, если удаление не удалось
            }

            return RedirectToPage("./Index");
        }
    }
}
