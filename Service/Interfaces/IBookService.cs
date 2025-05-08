using Domain;

namespace Service.Interfaces;

public interface IBookService : IService<Book>
{
    Task<IEnumerable<Book>> SearchAsync(string? title = null, string? author = null, List<string>? categories = null);
}