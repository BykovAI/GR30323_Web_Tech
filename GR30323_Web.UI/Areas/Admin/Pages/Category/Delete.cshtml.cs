using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GR30323_Web.Domain.Entities; // Пространство имен для Category
using GR30323_Web.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace GR30323_Web.UI.Areas.Admin.Pages.Category
{
    public class CategoryDeleteModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CategoryDeleteModel(GR30323_Web.UI.Data.AppDbContext context)
        {
            _context = context;
        }

        // Свойство для отображения данных категории
        [BindProperty]
        public GR30323_Web.Domain.Entities.Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загружаем категорию без связанных данных
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Удаляем только категорию
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
