using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Interfaces;

namespace Persistence.Repositories;

public class LendingRepository : ILendingRepository
{
    private readonly LibraryDbContext _context;

    public LendingRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Lending>> GetAllAsync()
    {
        return await _context.Lendings
            .Include(l => l.Book)
            .ToListAsync();
    }

    public void Add(Lending entity)
    {
        _context.Lendings.AddAsync(entity);
    }

    public void Update(Lending entity)
    {
        _context.Lendings.Update(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }
    }

    public async Task<Lending?> GetByIdAsync(int id)
    {
        return await _context.Lendings
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Lending>> GetByBookIdAsync(int bookId)
    {
        return await _context.Lendings
            .Where(l => l.BookId == bookId)
            .Include(l => l.Book)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lending>> GetByBorrowerAsync(string borrowerName)
    {
        return await _context.Lendings
            .Where(l => l.BorrowerName.ToLower() == borrowerName.ToLower())
            .Include(l => l.Book)
            .ToListAsync();
    }

    public async Task<Lending?> GetActiveLendingAsync(int bookId, string borrowerName)
    {
        return await _context.Lendings
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l =>
                l.BookId == bookId &&
                l.BorrowerName.ToLower() == borrowerName.ToLower() &&
                l.ReturnDate == null);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}