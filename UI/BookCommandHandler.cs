using System.CommandLine;
using System.Globalization;
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
                Console.WriteLine("Usage: book add --title \"...\" --author \"...\" --quantity <number> [--categories ...,...,]");
                return;
            }
            
            // validare input
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Error: Title is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("Error: Author is required.");
                return;
            }

            if (quantity < 0)
            {
                Console.WriteLine("Error: Quantity must be a non-negative number.");
                return;
            }
            
            // categorii
            args.TryGetValue("--categories", out var categoryListRaw);
            var categoryNames = categoryListRaw?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            using var scope = services.CreateScope();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();
            var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

            var categories = new List<Category>();

            foreach (var name in categoryNames)
            {
                
                var category = await categoryService.GetByNameAsync(NormalizeCategoryName(name));
                if (category == null)
                {
                    category = new Category { Name = name };
                    await categoryService.AddAsync(category);
                }
                categories.Add(category);
            }
            
            var existingBook = (await bookService.SearchAsync(title, author))
                .FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase)
                                     && b.Author.Equals(author, StringComparison.OrdinalIgnoreCase));

            if (existingBook != null)
            {
                existingBook.Quantity += quantity;
                await bookService.UpdateAsync(existingBook);
                Console.WriteLine($"Book already exists. Increased quantity to {existingBook.Quantity}.");
            }
            else
            {
                var book = new Book
                {
                    Title = title,
                    Author = author,
                    Quantity = quantity,
                    Categories = categories
                };

                await bookService.AddAsync(book);
                Console.WriteLine("Book added successfully.");
            }
        }

        private static async Task HandleListAsync(List<string> arguments,IServiceProvider services)
        {
            var args = ParseArguments(arguments);
            args.TryGetValue("--sortby", out var sortBy);
            args.TryGetValue("--title", out var titleFilter);
            args.TryGetValue("--author", out var authorFilter);
            args.TryGetValue("--categories", out var categoryRaw);
            var categoryNames = categoryRaw?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(NormalizeCategoryName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            
            using var scope = services.CreateScope();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

            IEnumerable<Book> books;
            if (!string.IsNullOrWhiteSpace(titleFilter) ||
                !string.IsNullOrWhiteSpace(authorFilter) ||
                categoryNames?.Any() == true)
            {
                books = await bookService.SearchAsync(titleFilter, authorFilter, categoryNames);
            }
            else
            {
                books = await bookService.GetAllAsync();
            }
            
            books = sortBy?.ToLower() switch
            {
                "title" => books.OrderBy(b => b.Title).ToList(),
                "author" => books.OrderBy(b => b.Author).ToList(),
                "rating" => books.OrderByDescending(b => b.AverageRating),
                "id" or null => books.OrderBy(b => b.Id).ToList(),
                _ => books.OrderBy(b => b.Id).ToList()
            };

            Console.WriteLine("\nBook List:");
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,5} | {4,5} | {5}", "ID", "Title", "Author", "Qty", "Rating", "Categories");
            Console.WriteLine("------------------------------------------------------------------------------------");

            foreach (var book in books)
            {
                var categories = book.Categories.Any()
                    ? string.Join(", ", book.Categories.Select(c => c.Name))
                    : "-";
                
                Console.WriteLine("{0,-5} | {1,-30} | {2,-20} | {3,5} | {4,5:F1} | {5}",
                    book.Id, book.Title, book.Author, book.Quantity, book.AverageRating, categories);
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
        
        private static string NormalizeCategoryName(string input)
        {
            input = input.Replace("-", " ");
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }