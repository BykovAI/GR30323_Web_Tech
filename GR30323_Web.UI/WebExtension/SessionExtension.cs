using System.Text.Json;

namespace GR30323_Web.UI.WebExtension
{
    public static class SessionExtension
    {
        // Сохранение объекта в сессию
        public static void Set<T>(this ISession session, string key, T item)
        {
            // Сериализация объекта в строку
            var serializedItem = JsonSerializer.Serialize(item);
            session.SetString(key, serializedItem);  // Сохранение сериализованной строки
        }

        // Получение объекта из сессии
        public static T Get<T>(this ISession session, string key)
        {
            var item = session.GetString(key);  // Получаем строку из сессии
            if (item == null)
            {
                // Возвращаем значение по умолчанию (default(T)) в случае отсутствия данных
                return default;
            }

            try
            {
                // Пробуем десериализовать строку обратно в объект
                return JsonSerializer.Deserialize<T>(item);
            }
            catch (JsonException)
            {
                // Если произошла ошибка десериализации, возвращаем значение по умолчанию
                return default;
            }
        }
    }
}
