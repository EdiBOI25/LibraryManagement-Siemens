using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                var dbPath = GetDatabasePath();
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
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

                logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
            });
    
    private static string GetDatabasePath()
    {
        var defaultPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "library.db");

        if (File.Exists(defaultPath))
            return defaultPath;
        Console.WriteLine("Please drag and drop your 'library.db' file here, then press Enter:");

        var inputPath = Console.ReadLine()?.Trim('"')?.Trim();

        while (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
        {
            Console.WriteLine("Invalid file path. Try again:");
            inputPath = Console.ReadLine()?.Trim('"')?.Trim();
        }

        return inputPath;
    }
}