using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GR30323_Web.Domain.Entities;
using GR30323_Web.UI.Data;
using GR30323_Web.Domain.Services.CategoryService;
using GR30323_Web.Domain.Services.ProductService;


namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class CarDetailsModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;
                private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CarDetailsModel(GR30323_Web.UI.Data.AppDbContext context, IProductService productService, ICategoryService categoryService)
        {
            _context = context;
            _productService = productService;
            _categoryService = categoryService;
        }

        public GR30323_Web.Domain.Entities.Car Car { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Проверяем, передан ли ID
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            //var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);


            var carResponse = await _productService.GetProductByIdAsync(id.Value);


            if (carResponse == null || !carResponse.Success || carResponse.Data == null)
            {
                return NotFound(); // Если автомобиль не найден, возвращаем 404
            }

            Car = carResponse.Data;

            // Если у автомобиля есть CategoryId, подгружаем категорию
            if (Car.CategoryId != 0)
            {
                var categoryResponse = await _categoryService.GetCategoryByIdAsync(Car.CategoryId);
                if (categoryResponse?.Data != null)
                {
                    Car.Category = categoryResponse.Data; // Присваиваем объект категории
                }
            }

            return Page(); // Возвращаем страницу
        }
    }
}
