using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GR30323_Web.Domain.Entities; // Пространство имен для Category
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Category
{
    public class CategoryCreateModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CategoryCreateModel(GR30323_Web.UI.Data.AppDbContext context)
        {
            _context = context;
        }

        // Свойство для привязки данных категории
        [BindProperty]
        public GR30323_Web.Domain.Entities.Category Category { get; set; } = default!;

        // Обработчик для отображения страницы (GET)
        public IActionResult OnGet()
        {
            return Page();
        }

        // Обработчик для обработки данных формы (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Добавляем новую категорию в базу данных
            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            // Перенаправляем на страницу списка категорий
            return RedirectToPage("./Index");
        }
    }
}
