using Domain;

namespace Persistence.Interfaces;

public interface ILendingRepository : IRepository<Lending>
{
    Task<IEnumerable<Lending>> GetByBookIdAsync(int bookId);
    Task<IEnumerable<Lending>> GetByBorrowerAsync(string borrowerName);
    Task<Lending?> GetActiveLendingAsync(int bookId, string borrowerName);

}