using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

var services = new ServiceCollection();

services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite("Data Source=library.db"));

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    
    var books = context.Books.ToList();
    Console.WriteLine($"Books in database: {books.Count}");
}