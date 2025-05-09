using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using UI;
using Persistence;
using Persistence.Interfaces;
using Persistence.Repositories;
using Service.Interfaces;
using Service.Services;


namespace LibraryManagement;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder().Build();
        var services = host.Services;

        await UIMain.RunAsync(services);
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var dbPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "library.db");
                var connectionString = $"Data Source={Path.GetFullPath(dbPath)}";
                Console.WriteLine(connectionString);
                
                services.AddDbContext<LibraryDbContext>(options =>
                    options.UseSqlite(connectionString));

                services.AddScoped<IBookRepository, BookRepository>();
                services.AddScoped<ILendingRepository, LendingRepository>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<IBookService, BookService>();
                services.AddScoped<ILendingService, LendingService>();
                services.AddScoped<ICategoryService, CategoryService>();
            });
}