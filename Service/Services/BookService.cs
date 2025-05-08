using Domain;
using Persistence.Interfaces;
using Service.Interfaces;

namespace Service.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Book>> SearchAsync(string? title = null, string? author = null, List<string>? categories = null)
    {
        return await _bookRepository.SearchAsync(title, author, categories);
    }

    public async Task AddAsync(Book book)
    {
        _bookRepository.Add(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _bookRepository.Update(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _bookRepository.DeleteAsync(id);
        await _bookRepository.SaveChangesAsync();
    }
}