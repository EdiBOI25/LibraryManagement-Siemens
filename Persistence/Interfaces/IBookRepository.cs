using Domain;

namespace Persistence.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> SearchAsync(string? title = null, string? author = null, List<string>? categories = null);
}