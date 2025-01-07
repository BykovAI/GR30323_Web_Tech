using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GR30323_Web.Domain.Entities; // ��� ������� � �������� Category
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages.Category
{
    public class CategoryIndexModel : PageModel
    {
        private readonly GR30323_Web.UI.Data.AppDbContext _context;

        public CategoryIndexModel(GR30323_Web.UI.Data.AppDbContext context)
        {
            _context = context;
        }

        // ������ ���������
        public IList<GR30323_Web.Domain.Entities.Category> Categories { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // �������� ��� ��������� �� ���� ������
            Categories = await _context.Categories.ToListAsync();
        }
    }
}
