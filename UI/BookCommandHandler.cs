using System.CommandLine;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace UI;

public static class BookCommandHandler
    {
        public static async Task HandleAsync(string subCommand, List<string> arguments, IServiceProvider services)
        {
            switch (subCommand)
            {
                case "add":
                    await HandleAddAsync(arguments, services);
                    break;

                case "list":
                    await HandleListAsync(arguments, services);
                    break;

                case "delete":
                    await HandleDeleteAsync(arguments, services);
                    break;

                default:
                    Console.WriteLine($"Unknown book command: '{subCommand}'");
                    break;
            }
        }

        private static async Task HandleAddAsync(List<string> arguments, IServiceProvider services)
        {
            var args = ParseArguments(arguments);

            if (!args.TryGetValue("--title", out var title) ||
                !args.TryGetValue("--author", out var author) ||
                !args.TryGetValue("--quantity", out var quantityStr) ||
                !int.TryParse(quantityStr, out var quantity))
            {
                Console.WriteLine("Usage: book add --title \"...\" --author \"...\" --quantity <number>");
                return;
            }

            var book = new Book
            {
                Title = title,
                Author = author,
                Quantity = quantity
            };

            using var scope = services.CreateScope();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

            await bookService.AddAsync(book);
            Console.WriteLine("Book added successfully.");
        }

        private static async Task HandleListAsync(List<string> arguments,IServiceProvider services)
        {
            var args = ParseArguments(arguments);
            args.TryGetValue("--sortby", out var sortBy);
            args.TryGetValue("--title", out var titleFilter);
            args.TryGetValue("--author", out var authorFilter);
            
            using var scope = services.CreateScope();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

            IEnumerable<Book> books;
            if (!string.IsNullOrWhiteSpace(titleFilter) || !string.IsNullOrWhiteSpace(authorFilter))
            {
                books = await bookService.SearchAsync(titleFilter, authorFilter);
            }
            else
            {
                books = await bookService.GetAllAsync();
            }
            
            books = sortBy?.ToLower() switch
            {
                "title" => books.OrderBy(b => b.Title).ToList(),
                "author" => books.OrderBy(b => b.Author).ToList(),
                "id" or null => books.OrderBy(b => b.Id).ToList(),
                _ => books.OrderBy(b => b.Id).ToList()
            };

            Console.WriteLine("\nBook List:");
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,5} | {4,5}", "ID", "Title", "Author", "Qty", "Rating");
            Console.WriteLine("------------------------------------------------------------------------------------");

            foreach (var book in books)
            {
                Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,5} | {4,5:F1}",
                    book.Id, book.Title, book.Author, book.Quantity, book.AverageRating);
            }

            Console.WriteLine("------------------------------------------------------------------------------------");
        }

        private static async Task HandleDeleteAsync(List<string> arguments, IServiceProvider services)
        {
            var args = ParseArguments(arguments);

            if (!args.TryGetValue("--id", out var idStr) || !int.TryParse(idStr, out var id))
            {
                Console.WriteLine("Usage: book delete --id <bookId>");
                return;
            }

            using var scope = services.CreateScope();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

            var existing = await bookService.GetByIdAsync(id);
            if (existing == null)
            {
                Console.WriteLine($"Book with ID {id} not found.");
                return;
            }

            await bookService.DeleteAsync(id);
            Console.WriteLine("Book deleted successfully.");
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