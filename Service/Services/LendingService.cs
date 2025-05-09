using Domain;
using Persistence.Interfaces;
using Service.Interfaces;

namespace Service.Services;

public class LendingService : ILendingService
{
    private readonly ILendingRepository _lendingRepository;
    private readonly IBookRepository _bookRepository;

    public LendingService(ILendingRepository lendingRepository, IBookRepository bookRepository)
    {
        _lendingRepository = lendingRepository;
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Lending>> GetAllAsync()
    {
        return await _lendingRepository.GetAllAsync();
    }

    public async Task AddAsync(Lending entity)
    {
        _lendingRepository.Add(entity);
        await _lendingRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lending entity)
    {
        _lendingRepository.Update(entity);
        await _lendingRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _lendingRepository.DeleteAsync(id);
        await _lendingRepository.SaveChangesAsync();
    }

    public async Task<Lending?> GetByIdAsync(int id)
    {
        return await _lendingRepository.GetByIdAsync(id);
    }

    public async Task<Lending?> GetActiveLendingAsync(int bookId, string borrowerName)
    {
        return await _lendingRepository.GetActiveLendingAsync(bookId, borrowerName);
    }

    public async Task<bool> BorrowBookAsync(int bookId, string borrowerName)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null || book.Quantity <= 0)
            return false;

        var existing = await _lendingRepository.GetActiveLendingAsync(bookId, borrowerName);
        if (existing != null)
            return false;

        var lending = new Lending
        {
            BookId = bookId,
            BorrowerName = borrowerName,
            BorrowDate = DateTime.Now
        };

        _lendingRepository.Add(lending);

        book.Quantity--;
        _bookRepository.Update(book);

        await _lendingRepository.SaveChangesAsync();
        await _bookRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ReturnBookAsync(int bookId, string borrowerName, int? rating = null)
    {
        var lending = await _lendingRepository.GetActiveLendingAsync(bookId, borrowerName);
        if (lending == null)
            return false;

        lending.ReturnDate = DateTime.Now;
        lending.Rating = rating;

        var book = lending.Book;
        if (book == null)
        {
            book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return false;
        }

        book.Quantity++;

        var allLendings = await _lendingRepository.GetByBookIdAsync(bookId);
        var ratings = allLendings
            .Where(l => l.Rating.HasValue)
            .Select(l => l.Rating!.Value)
            .ToList();

        if (ratings.Count > 0)
            book.AverageRating = (float)Math.Round(ratings.Average(), 1);

        _bookRepository.Update(book);
        await _lendingRepository.SaveChangesAsync();
        await _bookRepository.SaveChangesAsync();

        return true;
    }
}