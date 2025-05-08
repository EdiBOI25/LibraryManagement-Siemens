using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Lending> Lendings => Set<Lending>();
        public DbSet<Category> Categories => Set<Category>();

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(b => b.Author)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(b => b.Quantity)
                      .IsRequired();

                entity.Property(b => b.AverageRating)
                      .HasDefaultValue(0f);

                entity.HasMany(b => b.Categories)
                      .WithMany(c => c.Books);
            });

            
            modelBuilder.Entity<Lending>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.BorrowerName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(l => l.BorrowDate)
                      .IsRequired();

                entity.Property(l => l.Rating)
                      .HasDefaultValue(null);

                entity.HasOne(l => l.Book)
                      .WithMany(b => b.Lendings)
                      .HasForeignKey(l => l.BookId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(50);
            });
        }
    }