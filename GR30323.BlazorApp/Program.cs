using GR30323.BlazorApp.Components;
using GR30323_Blazor.Services;
using GR30323_Web.Domain.Entities;
using GR30323_Blazor.Services; // Добавьте пространство имен для вашего сервиса

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Настройка HttpClient для работы с API

builder.Services.AddHttpClient<IBlazorProductService<Car>, ApiBlazorProductService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7092/api/");
});



builder.Services.AddScoped<IBlazorProductService<Car>, ApiBlazorProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
