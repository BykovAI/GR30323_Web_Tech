using GR30323_Web.UI.Data;
using GR30323_Web.UI.Data.WebLabsV03.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GR30323_Web.Domain.Services.CategoryService;
using GR30323_Web.Domain.Services.ProductService;
using GR30323_Web.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using GR30323_Web.UI.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();



// ��������� Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// ���������� Serilog � ASP.NET Core
builder.Host.UseSerilog();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options =>
   {   
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
   }
)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p =>
    p.RequireClaim(ClaimTypes.Role, "admin"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("admin"));
});


builder.Services.AddRazorPages(options =>
{
    // ������������ ������ � ����� "Car" � ������� "Admin"
    options.Conventions.AuthorizeAreaFolder("Admin", "/Car", "AdminOnly");

    // ������������ ������ � ����� "Category" � ������� "Admin"
    options.Conventions.AuthorizeAreaFolder("Admin", "/Category", "AdminOnly");

    // ������������ ������ � ����������� ����� � ������� "Admin"
    options.Conventions.AuthorizeAreaPage("Admin", "/Index", "AdminOnly");
});


builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

// ������������ ICategoryService ��� scoped
builder.Services.AddScoped<ICategoryService, MemoryCategoryService>();

// ������������ IProductService ��� scoped
builder.Services.AddScoped<IProductService, MemoryProductService>();

// ����������� HttpClient ��� IProductService
builder.Services.AddHttpClient<IProductService, ApiProductService>(opt =>
    opt.BaseAddress = new Uri("https://localhost:7092/api/cars/"));

// ����������� HttpClient ��� ICategoryService
builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
    opt.BaseAddress = new Uri("https://localhost:7092/api/categories/"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // ��������� ������������ �������, ���� ����������
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ��������� Kestrel
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // ��������� ������ �� 10 ��
});

AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    var exception = args.ExceptionObject as Exception;
    File.AppendAllText("critical_log.txt", $"{DateTime.Now}: {exception?.Message}\n{exception?.StackTrace}\n");
};

TaskScheduler.UnobservedTaskException += (sender, args) =>
{
    File.AppendAllText("critical_log.txt", $"{DateTime.Now}: {args.Exception.Message}\n{args.Exception.StackTrace}\n");
    args.SetObserved();
};


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ��������� middleware
app.UseFileLogger();

app.MapRazorPages();

await DbInit.SeedData(app);

app.Run();
