using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence
{
    public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
    {
        public LibraryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
            optionsBuilder.UseSqlite("Data Source=library.db");

            return new LibraryDbContext(optionsBuilder.Options);
        }
    }
}