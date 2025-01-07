using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GR30323_Web.UI.Areas.Admin.Pages.Car
{
    public class TestUploadModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public TestUploadModel(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? uploadFile)
        {
            try
            {
                if (uploadFile == null)
                {
                    return Content("Файл не был загружен!");
                }

                return Content($"Файл принят: {uploadFile.FileName}, {uploadFile.Length} байт");
            }
            catch (Exception ex)
            {
                return Content($"Произошла ошибка: {ex.Message}");
            }
        }



    }
}
