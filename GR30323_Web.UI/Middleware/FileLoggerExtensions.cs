using Microsoft.AspNetCore.Builder;


namespace GR30323_Web.UI.Middleware
{
    public static class FileLoggerExtensions
    {
        public static IApplicationBuilder UseFileLogger(this IApplicationBuilder builder)
        {
            // Регистрируем FileLogger middleware в pipeline
            return builder.UseMiddleware<FileLogger>();
        }
    }
}
