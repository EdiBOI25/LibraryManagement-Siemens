using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Interfaces;

namespace Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.Categories)
            .ToListAsync();
    }

    public void Add(Book book)
    {
        _context.Books.AddAsync(book);
    }

    public void Update(Book book)
    {
        _context.Books.Update(book);
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> SearchAsync(string? title = null, string? author = null, List<string>? categories = null)
    {
        var query = _context.Books
            .Include(b => b.Categories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (categories != null && categories.Count > 0)
        {
            query = query.Where(b => b.Categories.Any(c => categories.Contains(c.Name)));
        }

        return await query.ToListAsync();
    }
}