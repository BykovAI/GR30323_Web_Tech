using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GR30323_Web.Domain.Entities; // Пространство имен для Category
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Category
{
    public class CategoryEditModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CategoryEditModel(GR30323_Web.UI.Data.AppDbContext context)
        {
            _context = context;
        }

        // Свойство для привязки данных категории
        [BindProperty]
        public GR30323_Web.Domain.Entities.Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загружаем категорию по ID
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверяем существование категории перед обновлением
            var existingCategory = await _context.Categories.FindAsync(Category.Id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            // Обновляем данные категории
            existingCategory.Name = Category.Name;
            existingCategory.NormalizedName = Category.NormalizedName;

            await _context.SaveChangesAsync();

            // Перенаправляем на список категорий после обновления
            return RedirectToPage("./Index");
        }
    }
}
