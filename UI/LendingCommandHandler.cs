using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace UI;

public static class LendingCommandHandler
{
    public static async Task HandleAsync(string subCommand, List<string> arguments, IServiceProvider services)
    {
        switch (subCommand)
        {
            case "borrow":
                await HandleBorrowAsync(arguments, services);
                break;
            
            case "return":
                await HandleReturnAsync(arguments, services);
                break;
            
            case "list":
                await HandleListAsync(arguments, services);
                break;


            default:
                Console.WriteLine($"Unknown lending command: '{subCommand}'");
                break;
        }
    }

    private static async Task HandleBorrowAsync(List<string> arguments, IServiceProvider services)
    {
        var args = ParseArguments(arguments);

        if (!args.TryGetValue("--bookid", out var bookIdStr) || !int.TryParse(bookIdStr, out var bookId))
        {
            Console.WriteLine("Usage: lending borrow --bookid n --name \"...\"");
            return;
        }

        if (!args.TryGetValue("--name", out var borrowerName) || string.IsNullOrWhiteSpace(borrowerName))
        {
            Console.WriteLine("Error: Borrower name is required.");
            return;
        }

        using var scope = services.CreateScope();
        var lendingService = scope.ServiceProvider.GetRequiredService<ILendingService>();

        var success = await lendingService.BorrowBookAsync(bookId, borrowerName);
        if (success)
        {
            Console.WriteLine("Book borrowed successfully.");
        }
        else
        {
            Console.WriteLine("Cannot borrow book. Either it's out of stock or already borrowed by this person.");
        }
    }
    
    private static async Task HandleReturnAsync(List<string> arguments, IServiceProvider services)
    {
        var args = ParseArguments(arguments);

        if (!args.TryGetValue("--bookid", out var bookIdStr) || !int.TryParse(bookIdStr, out var bookId))
        {
            Console.WriteLine("Usage: lending return --bookid n --name \"...\" [--rating 1-10]");
            return;
        }

        if (!args.TryGetValue("--name", out var borrowerName) || string.IsNullOrWhiteSpace(borrowerName))
        {
            Console.WriteLine("Error: Borrower name is required.");
            return;
        }

        int? rating = null;
        if (args.TryGetValue("--rating", out var ratingStr))
        {
            if (int.TryParse(ratingStr, out var parsedRating))
            {
                if (parsedRating < 1 || parsedRating > 10)
                {
                    Console.WriteLine("Error: Rating must be between 1 and 10.");
                    return;
                }
                rating = parsedRating;
            }
            else
            {
                Console.WriteLine("Error: Rating must be an integer.");
                return;
            }
        }

        using var scope = services.CreateScope();
        var lendingService = scope.ServiceProvider.GetRequiredService<ILendingService>();

        var success = await lendingService.ReturnBookAsync(bookId, borrowerName, rating);
        if (success)
        {
            Console.WriteLine("Book returned successfully.");
        }
        else
        {
            Console.WriteLine("No active lending found for this user and book.");
        }
    }

    private static async Task HandleListAsync(List<string> arguments, IServiceProvider services)
    {
        var args = ParseArguments(arguments);

        args.TryGetValue("--name", out var borrowerFilter);
        int? bookIdFilter = null;

        if (args.TryGetValue("--bookid", out var bookIdStr) && int.TryParse(bookIdStr, out var bookId))
        {
            bookIdFilter = bookId;
        }
        
        bool activeOnly = arguments.Contains("--active");

        using var scope = services.CreateScope();
        var lendingService = scope.ServiceProvider.GetRequiredService<ILendingService>();

        var lendings = await lendingService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(borrowerFilter))
        {
            lendings = lendings
                .Where(l => l.BorrowerName.Equals(borrowerFilter, StringComparison.OrdinalIgnoreCase));
        }
        
        if (activeOnly)
        {
            lendings = lendings.Where(l => l.ReturnDate == null);
        }

        if (bookIdFilter.HasValue)
        {
            lendings = lendings.Where(l => l.BookId == bookIdFilter.Value);
        }

        if (!lendings.Any())
        {
            Console.WriteLine("No lending records found.");
            return;
        }

        Console.WriteLine("\nLending List:");
        Console.WriteLine("------------------------------------------------------------------------------------------------------");
        Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,-20} | {4,-20} | {5,5}", 
            "ID", "Book Title", "Borrower", "Borrow Date", "Return Date", "Rating");
        Console.WriteLine("------------------------------------------------------------------------------------------------------");

        foreach (var l in lendings)
        {
            var returnDate = l.ReturnDate?.ToString("yyyy-MM-dd") ?? "-";
            var rating = l.Rating?.ToString() ?? "-";

            Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,-20} | {4,-20} | {5,5}",
                l.Id, l.Book?.Title ?? "(n/a)", l.BorrowerName, l.BorrowDate.ToString("yyyy-MM-dd"), returnDate, rating);
        }

        Console.WriteLine("------------------------------------------------------------------------------------------------------");
    }
    
    private static Dictionary<string, string> ParseArguments(List<string> tokens)
    {
        var result = new Dictionary<string, string>();
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i].StartsWith("--") && !tokens[i + 1].StartsWith("--"))
            {
                result[tokens[i]] = tokens[i + 1];
                i++;
            }
        }
        return result;
    }
}