using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Models;
using GR30323_Web.Domain.Services.CategoryService;

namespace GR30323_Web.UI.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7092/");
        }

        /* public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
         {
             var response = await _httpClient.GetAsync("api/Categories");

             if (!response.IsSuccessStatusCode)
                 return new ResponseData<List<Category>> { Success = false, ErrorMessage = "Ошибка при получении категорий" };

             var responseString = await response.Content.ReadAsStringAsync();
             var result = JsonSerializer.Deserialize<ResponseData<List<Category>>>(responseString, new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             });

             return result!;
         }*/

     /*   public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Categories");

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<List<Category>>
                    {
                        Success = false,
                        ErrorMessage = $"Ошибка при получении категорий: {response.StatusCode}"
                    };
                }

                // Десериализация как простой список категорий
                var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
                return new ResponseData<List<Category>>
                {
                    Success = true,
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }*/

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
{
    try
    {
        var response = await _httpClient.GetAsync("api/Categories");

        if (!response.IsSuccessStatusCode)
        {
            return new ResponseData<List<Category>>
            {
                Success = false,
                ErrorMessage = $"Ошибка при получении категорий: {response.StatusCode} - {response.ReasonPhrase}"
            };
        }

        var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
        return new ResponseData<List<Category>>
        {
            Success = true,
            Data = categories
        };
    }
    catch (Exception ex)
    {
        return new ResponseData<List<Category>>
        {
            Success = false,
            ErrorMessage = $"Ошибка: {ex.Message}"
        };
    }
}


        /*  public async Task<ResponseData<Category>> GetCategoryByIdAsync(int categoryId)
          {
              // Формируем URL для запроса, используя categoryId
              var url = $"api/Categories/{categoryId}";

              try
              {
                  // Выполняем запрос
                  var response = await _httpClient.GetFromJsonAsync<ResponseData<Category>>(url);

                  // Возвращаем результат или сообщение об ошибке, если ответ пустой
                  return response ?? new ResponseData<Category> { Success = false, ErrorMessage = "Category not found." };
              }
              catch (Exception ex)
              {
                  // Логируем ошибку, если запрос не удался
                  return new ResponseData<Category> { Success = false, ErrorMessage = ex.Message };
              }
          }
        */
        public async Task<ResponseData<Category>> GetCategoryByIdAsync(int categoryId)
        {
            var url = $"api/Categories/{categoryId}";

            try
            {
                // Отправляем запрос на API
                var response = await _httpClient.GetFromJsonAsync<Category>(url);

                // Если данные успешно получены, возвращаем их
                if (response != null)
                {
                    return new ResponseData<Category>
                    {
                        Success = true,
                        Data = response
                    };
                }

                // Если ответ null, возвращаем ошибку
                return new ResponseData<Category>
                {
                    Success = false,
                    ErrorMessage = "Category not found."
                };
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, возвращаем сообщение об ошибке
                return new ResponseData<Category>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }





    }
}
