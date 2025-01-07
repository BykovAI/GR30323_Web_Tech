using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GR30323_Web.Domain.Entities;
using GR30323_Web.UI.Data;

namespace GR30323_Web.UI.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Логика загрузки данных для главной страницы админки
            // Например, можно загрузить статистику или информацию о текущих сущностях
        }
    }
}
