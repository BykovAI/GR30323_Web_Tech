using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Services.ProductService;
using GR30323_Web.Domain.Services.CategoryService;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class CarIndexModel : PageModel
    {
        private readonly IProductService _productService;  // Инжекция сервиса вместо контекста
        private readonly ICategoryService _categoryService;  // Инжекция сервиса вместо контекста

        private readonly GR30323_Web.UI.Data.AppDbContext _context;


        public CarIndexModel(IProductService productService, ICategoryService categoryService, GR30323_Web.UI.Data.AppDbContext context)  // Замена конструктора
        {
            _productService = productService;
            _categoryService = categoryService;
            _context = context;
        }

        public IList<GR30323_Web.Domain.Entities.Car> Cars { get; set; } = default!;
        public IList<GR30323_Web.Domain.Entities.Category> Categories { get; set; } = default!;

        //public async Task OnGetAsync()
        //{
        //   var response = await _productService.GetProductListAsync(null, 1);


        //    if (response != null && response.Success && response.Data != null)
        //    {
        //        Console.WriteLine("Data retrieved successfully from API.");
        //        Cars = response.Data.Items;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Failed to retrieve data from API.");
        //        Cars = new List<GR30323_Web.Domain.Entities.Car>(); // Инициализация пустого списка
        //    }

        //    // Проверка на случай, если внутри элементов Cars свойства Category остаются null
        //    foreach (var car in Cars)
        //    {
        //        if (car.Category == null)
        //        {
        //            car.Category = new GR30323_Web.Domain.Entities.Category
        //            {
        //                Name = "Unknown"
        //            };
        //        }
        //    }
        //}
        public async Task OnGetAsync()
        {
            // Получаем список автомобилей
            var response = await _productService.GetProductListAsync(null, 1);

            if (response != null && response.Success && response.Data != null)
            {
                Console.WriteLine("Data retrieved successfully from API.");
                Cars = response.Data.Items;

                // Для каждого автомобиля делаем запрос для получения категории
                foreach (var car in Cars)
                {
                    // Получаем категорию напрямую из базы по CategoryId
                    //var category = await _context.Categories.FindAsync(car.CategoryId);

                    var categoryResponse = await _categoryService.GetCategoryByIdAsync(car.CategoryId);
                    var category = categoryResponse?.Data;

                    // Присваиваем категорию автомобилю
                    car.Category = category;
                }

            }
            else
            {
                Console.WriteLine("Failed to retrieve data from API.");
                Cars = new List<GR30323_Web.Domain.Entities.Car>(); // Инициализация пустого списка
            }

             

        }

    }



}




/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GR30323_Web.Domain.Entities;
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class CarIndexModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CarIndexModel(GR30323_Web.UI.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<GR30323_Web.Domain.Entities.Car> Cars { get; set; } = default!;
        public IList<GR30323_Web.Domain.Entities.Category> Categories { get; set; } = default!;


        public async Task OnGetAsync()
        {
            Cars = await _context.Cars
                .Include(c => c.Category)
                .ToListAsync();
        }
    }
}*/
