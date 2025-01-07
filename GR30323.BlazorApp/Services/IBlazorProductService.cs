using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR30323_Blazor.Services
{
    public interface IBlazorProductService<T> where T : class
    {
        // Событие изменения списка
        event Action ListChanged;

        // Список объектов
        IEnumerable<T> Items { get; }

        // Номер текущей страницы
        int CurrentPage { get; }

        // Общее количество страниц
        int TotalPages { get; }

        // Получение списка объектов
        Task GetProducts(int pageNo = 1, int pageSize = 3);

        // Сообщение о статусе
        string StatusMessage { get; }
    }
}
