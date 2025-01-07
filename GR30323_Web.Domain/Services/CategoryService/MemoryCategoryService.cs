using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GR30323_Web.Domain.Services.CategoryService
{
    public class MemoryCategoryService : ICategoryService
    {
        private readonly List<Category> _categories = new()
        {
            new Category { Id = 1, Name = "Седаны", NormalizedName = "sedans" },
            new Category { Id = 2, Name = "Внедорожники", NormalizedName = "SUVs" },
            new Category { Id = 3, Name = "Электромобили", NormalizedName = "electric-cars" }
        };

        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            return Task.FromResult(new ResponseData<List<Category>> { Data = _categories });
        }

        // Получение категории по ID
        public Task<ResponseData<Category>> GetCategoryByIdAsync(int categoryId)
        {
            // Находим категорию по ID
            var category = _categories.FirstOrDefault(c => c.Id == categoryId);

            // Возвращаем результат
            if (category != null)
            {
                return Task.FromResult(new ResponseData<Category> { Data = category });
            }

            return Task.FromResult(new ResponseData<Category>
            {
                Success = false,
                ErrorMessage = "Category not found."
            });
        }


    }
}
