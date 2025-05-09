using Domain;

namespace Service.Interfaces;

public interface ILendingService : IService<Lending>
{
    Task<Lending?> GetActiveLendingAsync(int bookId, string borrowerName);
    Task<bool> BorrowBookAsync(int bookId, string borrowerName);
    Task<bool> ReturnBookAsync(int bookId, string borrowerName, int? rating = null);
}