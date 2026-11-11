using LibraryManagementAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.DataAccess
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Jane", LastName = "Austen", YearOfBirth = 1775 },
                new Author { Id = 2, FirstName = "Mark", LastName = "Twain", YearOfBirth = 1835 },
                new Author { Id = 3, FirstName = "Virginia", LastName = "Woolf", YearOfBirth = 1882 },
                new Author { Id = 4, FirstName = "George", LastName = "Orwell", YearOfBirth = 1903 },
                new Author { Id = 5, FirstName = "Toni", LastName = "Morrison", YearOfBirth = 1931 },
                new Author { Id = 6, FirstName = "Gabriel", LastName = "Marquez", YearOfBirth = 1927 },
                new Author { Id = 7, FirstName = "Mary", LastName = "Shelley", YearOfBirth = 1797 },
                new Author { Id = 8, FirstName = "Leo", LastName = "Tolstoy", YearOfBirth = 1828 },
                new Author { Id = 9, FirstName = "Agatha", LastName = "Christie", YearOfBirth = 1890 },
                new Author { Id = 10, FirstName = "James", LastName = "Baldwin", YearOfBirth = 1924 });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "test" },
                new Category { Id = 2, Name = "AC" },
                new Category { Id = 3, Name = "Fiber" });
        }
    }
}
