using Microsoft.Extensions.DependencyInjection;

namespace UI;

public static class UIMain
{
    public static async Task RunAsync(IServiceProvider services)
    {
        Console.WriteLine("-------------------------");
        Console.WriteLine("LIBRARY MANAGEMENT SYSTEM");
        Console.WriteLine("-------------------------\n");
        Console.WriteLine("Type 'help' to list available commands. Type 'exit/quit' to quit.\n");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var command = input.Trim();

            if (command.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                command.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            try
            {
                using var scope = services.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                await CommandInterpreter.ExecuteAsync(command, serviceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine("Bye!");
    }
}