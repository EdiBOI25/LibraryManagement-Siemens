using Domain;

namespace Service.Interfaces;

public interface ICategoryService : IService<Category>
{
    Task<Category?> GetByNameAsync(string name);
}