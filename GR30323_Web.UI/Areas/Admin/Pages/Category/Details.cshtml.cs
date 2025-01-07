using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GR30323_Web.Domain.Entities; // Пространство имен для Category
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Category
{
    public class CategoryDetailsModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CategoryDetailsModel(GR30323_Web.UI.Data.AppDbContext context)
        {
            _context = context;
        }

        // Свойство для отображения данных категории
        public GR30323_Web.Domain.Entities.Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Загрузка данных категории по ID
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            Category = category;
            return Page();
        }
    }
}
