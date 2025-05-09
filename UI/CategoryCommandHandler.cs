using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace UI;

public static class CategoryCommandHandler
{
    public static async Task HandleAsync(string subCommand, List<string> arguments, IServiceProvider services)
    {
        switch (subCommand)
        {
            case "list":
                await HandleListAsync(services);
                break;

            default:
                Console.WriteLine($"Unknown category command: '{subCommand}'");
                break;
        }
    }

    private static async Task HandleListAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

        var categories = await categoryService.GetAllAsync();

        if (!categories.Any())
        {
            Console.WriteLine("No categories found.");
            return;
        }

        Console.WriteLine("\nCategory List:");
        Console.WriteLine("-------------------------------");
        Console.WriteLine("{0,-5} | {1,-20}", "ID", "Name");
        Console.WriteLine("-------------------------------");

        foreach (var category in categories)
        {
            Console.WriteLine("{0,-5} | {1,-20}", category.Id, category.Name);
        }

        Console.WriteLine("-------------------------------");
    }
}