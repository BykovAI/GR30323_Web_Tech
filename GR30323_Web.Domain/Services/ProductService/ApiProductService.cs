using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GR30323_Web.Domain.Services.ProductService
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7092/");
        }

        //public async Task<ResponseData<ListModel<Car>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        //{
        //    var response = await _httpClient.GetFromJsonAsync<ResponseData<ListModel<Car>>>($"api/cars?category={categoryNormalizedName}&page={pageNo}");
        //    return response ?? new ResponseData<ListModel<Car>> { Success = false, ErrorMessage = "Error fetching cars." };
        //}


        public async Task<ResponseData<ListModel<Car>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            // Формируем URL в зависимости от того, есть ли значение для categoryNormalizedName
            var url = string.IsNullOrEmpty(categoryNormalizedName)
                ? $"api/cars?page={pageNo}"
                : $"api/cars?category={categoryNormalizedName}&page={pageNo}";

            // Выполняем запрос
            var response = await _httpClient.GetFromJsonAsync<ResponseData<ListModel<Car>>>(url);

            // Возвращаем результат или сообщение об ошибке, если ответ пустой
            return response ?? new ResponseData<ListModel<Car>> { Success = false, ErrorMessage = "Error fetching cars." };
        }


        //public async Task<ResponseData<ListModel<Car>>> GetAllCarsAsync()
        //{
        //    // Запрос к API для получения всех машин без фильтров
        //    var response = await _httpClient.GetFromJsonAsync<ResponseData<ListModel<Car>>>("api/cars");

        //    // Убедимся, что response не null и данные есть
        //    if (response?.Data?.Items != null)
        //    {
        //        // Если машины найдены, возвращаем их
        //        return response;
        //    }
        //    else
        //    {
        //        // Если данные не найдены, возвращаем пустой список
        //        return new ResponseData<ListModel<Car>> { Success = false, ErrorMessage = "No cars found." };
        //    }
        //}


        /*
        public async Task<ResponseData<Car>> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ResponseData<Car>>($"api/cars/{id}");
            return response ?? new ResponseData<Car> { Success = false, ErrorMessage = "Car not found." };
        }
        */

        public async Task<ResponseData<Car>> GetProductByIdAsync(int id)
        {
            var url = $"api/cars/{id}";

            try
            {
                // Десериализуем ответ напрямую в объект Car
                var car = await _httpClient.GetFromJsonAsync<Car>(url);

                if (car != null)
                {
                    return new ResponseData<Car>
                    {
                        Success = true,
                        Data = car
                    };
                }

                return new ResponseData<Car>
                {
                    Success = false,
                    ErrorMessage = "Car not found."
                };
            }
            catch (HttpRequestException ex)
            {
                return new ResponseData<Car>
                {
                    Success = false,
                    ErrorMessage = $"HTTP error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<Car>
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}"
                };
            }
        }




        public async Task<ResponseData<Car>> AddCarAsync(Car car, IFormFile? image)
        {
            var response = await _httpClient.PostAsJsonAsync("api/cars", car);
            if (response.IsSuccessStatusCode)
            {
                var addedCar = await response.Content.ReadFromJsonAsync<Car>();
                return new ResponseData<Car> { Success = true, Data = addedCar };
            }
            return new ResponseData<Car> { Success = false, ErrorMessage = "Error adding car." };
        }

        public async Task<ResponseData<Car>> UpdateCarAsync(Car car, IFormFile? image)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/cars/{car.Id}", car);
            if (response.IsSuccessStatusCode)
            {
                var updatedCar = await response.Content.ReadFromJsonAsync<Car>();
                return new ResponseData<Car> { Success = true, Data = updatedCar };
            }
            return new ResponseData<Car> { Success = false, ErrorMessage = "Error updating car." };
        }

        /*
        public async Task<ResponseData<Car>> UpdateProductAsync(Car car)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/cars/{car.Id}", car);
            if (response.IsSuccessStatusCode)
            {
                var updatedCar = await response.Content.ReadFromJsonAsync<Car>();
                return new ResponseData<Car> { Success = true, Data = updatedCar };
            }
            return new ResponseData<Car> { Success = false, ErrorMessage = "Error updating car." };
        }*/


        public async Task<ResponseData<Car>> UpdateProductAsync(Car car)
        {
            try
            {
                // Отправляем запрос на обновление автомобиля
                var response = await _httpClient.PutAsJsonAsync($"api/cars/{car.Id}", car);

                // Если запрос неуспешен, возвращаем ошибку
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<Car>
                    {
                        Success = false,
                        ErrorMessage = $"Ошибка при обновлении: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }

                // Проверяем, есть ли тело ответа
                if (response.Content.Headers.ContentLength == 0)
                {
                    return new ResponseData<Car>
                    {
                        Success = true,
                        Data = null // Указываем, что обновление прошло успешно, но данных нет
                    };
                }

                // Десериализуем ответ, если он существует
                var updatedCar = await response.Content.ReadFromJsonAsync<Car>();

                return new ResponseData<Car>
                {
                    Success = true,
                    Data = updatedCar
                };
            }
            catch (Exception ex)
            {
                // Обрабатываем исключения
                return new ResponseData<Car>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<ResponseData<Car>> CreateProductAsync(Car car)
        {
            try
            {
                // Отправка POST-запроса с JSON-данными
                var response = await _httpClient.PostAsJsonAsync("api/cars", car);

                // Проверка успешности ответа
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<Car>
                    {
                        Success = false,
                        ErrorMessage = $"Ошибка при создании автомобиля: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }

                // Десериализация ответа
                var createdCar = await response.Content.ReadFromJsonAsync<Car>();
                return new ResponseData<Car>
                {
                    Success = true,
                    Data = createdCar
                };
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                return new ResponseData<Car>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка: {ex.Message}"
                };
            }
        }



        public async Task<ResponseData<Car>> DeleteCarAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/cars/{id}");
            if (response.IsSuccessStatusCode)
            {
                return new ResponseData<Car> { Success = true };
            }
            return new ResponseData<Car> { Success = false, ErrorMessage = "Error deleting car." };
        }



    }
}
