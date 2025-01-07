using GR30323_Blazor.Services;
using GR30323_Web.Domain.Entities;
using GR30323_Web.Domain.Models;

public class ApiBlazorProductService : IBlazorProductService<Car>
{
    public ApiBlazorProductService(HttpClient httpClient)
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient), "HttpClient не передан в ApiBlazorProductService.");
        }

        _httpClient = httpClient;
    }

    private readonly HttpClient _httpClient;

    private List<Car> _cars = new();
    private int _currentPage = 1;
    private int _totalPages = 1;

    public IEnumerable<Car> Items => _cars;
    public int CurrentPage => _currentPage;
    public int TotalPages => _totalPages;

    public string StatusMessage { get; private set; } = string.Empty; // Реализация свойства

    public event Action ListChanged;


    public async Task GetProducts(int pageNo = 1, int pageSize = 3)
    {
        try
        {
            // Формируем полный URL запроса
            var utlik = "https://localhost:7092/api/cars";
            var query = $"{utlik}?pageNo={pageNo}&pageSize={pageSize}";

            // Логируем запрос
            AddStatusMessage($"Запрос отправлен: {query}");

            // Выполняем запрос к API
            var response = await _httpClient.GetFromJsonAsync<ResponseData<ListModel<Car>>>(query);

            if (response != null && response.Success)
            {
                // Успешный ответ, обновляем свойства
                _cars = response.Data.Items;
                _currentPage = response.Data.CurrentPage;
                _totalPages = response.Data.TotalPages;

                AddStatusMessage($"Успешно загружено объектов: {_cars.Count}");
            }
            else
            {
                // Обработка ошибки
                _cars = new List<Car>();
                _currentPage = 1;
                _totalPages = 1;

                AddStatusMessage($"Ошибка загрузки данных: {response?.ErrorMessage ?? "Неизвестная ошибка"}");
            }

            ListChanged?.Invoke(); // Уведомляем подписчиков об изменении данных
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            AddStatusMessage($"Ошибка: {ex.Message}");
            _cars = new List<Car>();
            _currentPage = 1;
            _totalPages = 1;
        }
    }


    private void AddStatusMessage(string message)
    {
        StatusMessage += $"{message}\n"; // Конкатенация сообщений
        ListChanged?.Invoke(); // Уведомление об изменении для обновления интерфейса
    }

}
