using ErrorOr;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.Errors;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.RequestParameters.Enums;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class BookService(LibraryDbContext libraryDbContext, BookMapper bookMapper) : IBookService
    {
        public async Task<ErrorOr<List<ReadBookDto>>> Get(PaginationParameters pagination, BookFilteringParameters bookFilteringParameters, SortingParameters<BookSortBy> sortingParameters)
        {
            var query = libraryDbContext.Books
                .Include(book => book.Category)
                .Include(book => book.Authors)
                .AsQueryable();

            if (bookFilteringParameters.Title != null)
            {
                query = query.Where(book => book.Title.Contains(bookFilteringParameters.Title));
            }

            if (bookFilteringParameters.CategoryId != null)
            {
                query = query.Where(book => book.CategoryId == bookFilteringParameters.CategoryId);
            }

            if (bookFilteringParameters.AuthorId != null)
            {
                query = query.Where(book => book.Authors.Any(author => author.Id == bookFilteringParameters.AuthorId));
            }

            query = sortingParameters.SortBy switch
            {
                BookSortBy.Title => sortingParameters.SortDirection == SortDirection.Desc ? query.OrderByDescending(book => book.Title) : query.OrderBy(book => book.Title),
                BookSortBy.AvailableCopies => sortingParameters.SortDirection == SortDirection.Desc ? query.OrderByDescending(book => book.AvailableCopies) : query.OrderBy(book => book.AvailableCopies),
                BookSortBy.TotalCopies => sortingParameters.SortDirection == SortDirection.Desc ? query.OrderByDescending(book => book.TotalCopies) : query.OrderBy(book => book.TotalCopies),
                _ => sortingParameters.SortDirection == SortDirection.Desc ? query.OrderByDescending(book => book.Id) : query.OrderBy(book => book.Id),
            };


            var books = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var totalBooks = await libraryDbContext.Books.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalBooks / pagination.PageSize);
            return books.Select(bookMapper.ToReadDto).ToList();
        }
        public async Task<ErrorOr<ReadBookDto>> CreateBookAsync(CreateBookDto bookDto)
        {
            var category = await libraryDbContext.Categories.FirstOrDefaultAsync(category => category.Id == bookDto.CategoryId);

            if (category is null)
            {
                return BookErrors.CategoryNotFound(bookDto.CategoryId);
            }
            var authors = await libraryDbContext.Authors
                .Where(author => bookDto.AuthorIds.Contains(author.Id))
                .ToListAsync();
            if (authors.Count != bookDto.AuthorIds.Count)
            {
                return BookErrors.AuthorsNotFound();
            }
            var book = bookMapper.ToEntity(bookDto);
            book.Category = category;
            book.Authors = authors;
            book.AvailableCopies = bookDto.TotalCopies;
            libraryDbContext.Books.Add(book);
            await libraryDbContext.SaveChangesAsync();

            return bookMapper.ToReadDto(book);
        }
    }
}
