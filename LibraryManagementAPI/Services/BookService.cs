using ErrorOr;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.Errors;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.RequestParameters.Enums;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services;

public sealed class BookService(LibraryDbContext dbContext, BookMapper bookMapper) : IBookService
{
    public async Task<ErrorOr<PagedResult<ReadBookDto>>> GetAsync(
        PaginationParameters pagination,
        BookFilteringParameters filtering,
        SortingParameters<BookSortBy> sorting)
    {
        var query = dbContext.Books
            .Include(book => book.Category)
            .Include(book => book.Authors)
            .AsQueryable();

        if (filtering.Title is not null)
        {
            query = query.Where(book => book.Title.Contains(filtering.Title));
        }

        if (filtering.CategoryId is not null)
        {
            query = query.Where(book => book.CategoryId == filtering.CategoryId);
        }

        if (filtering.AuthorId is not null)
        {
            query = query.Where(book => book.Authors.Any(author => author.Id == filtering.AuthorId));
        }

        query = sorting.SortBy switch
        {
            BookSortBy.Title => sorting.SortDirection == SortDirection.Desc
                ? query.OrderByDescending(book => book.Title)
                : query.OrderBy(book => book.Title),
            BookSortBy.AvailableCopies => sorting.SortDirection == SortDirection.Desc
                ? query.OrderByDescending(book => book.AvailableCopies)
                : query.OrderBy(book => book.AvailableCopies),
            BookSortBy.TotalCopies => sorting.SortDirection == SortDirection.Desc
                ? query.OrderByDescending(book => book.TotalCopies)
                : query.OrderBy(book => book.TotalCopies),
            _ => sorting.SortDirection == SortDirection.Desc
                ? query.OrderByDescending(book => book.Id)
                : query.OrderBy(book => book.Id)
        };

        var totalCount = await query.CountAsync();
        var books = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var items = books.Select(bookMapper.ToReadDto).ToList();
        var totalPages = (int)Math.Ceiling((double)totalCount / pagination.PageSize);
        return new PagedResult<ReadBookDto>(items, totalCount, totalPages);
    }

    public async Task<ErrorOr<ReadBookDto>> GetByIdAsync(int id)
    {
        var book = await dbContext.Books
            .Include(book => book.Category)
            .Include(book => book.Authors)
            .FirstOrDefaultAsync(book => book.Id == id);
        if (book is null)
        {
            return BookErrors.BookNotFound(id);
        }

        return bookMapper.ToReadDto(book);
    }

    public async Task<ErrorOr<ReadBookDto>> CreateAsync(CreateBookDto request)
    {
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == request.CategoryId);
        if (category is null)
        {
            return BookErrors.CategoryNotFound(request.CategoryId);
        }

        var authors = await dbContext.Authors
            .Where(author => request.AuthorIds.Contains(author.Id))
            .ToListAsync();
        if (authors.Count != request.AuthorIds.Count)
        {
            return BookErrors.AuthorsNotFound();
        }

        var book = bookMapper.ToEntity(request);
        book.Category = category;
        book.Authors = authors;
        book.AvailableCopies = request.TotalCopies;
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();
        return bookMapper.ToReadDto(book);
    }

    public async Task<ErrorOr<ReadBookDto>> UpdateAsync(int id, UpdateBookDto request)
    {
        var book = await dbContext.Books
            .Include(book => book.Category)
            .Include(book => book.Authors)
            .FirstOrDefaultAsync(book => book.Id == id);
        if (book is null)
        {
            return BookErrors.BookNotFound(id);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BookErrors.TitleRequired;
        }

        if (await dbContext.Books.AnyAsync(existing => existing.Id != id && existing.Title == request.Title))
        {
            return BookErrors.DuplicateTitle;
        }

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == request.CategoryId);
        if (category is null)
        {
            return BookErrors.CategoryNotFound(request.CategoryId);
        }

        var authors = await dbContext.Authors
            .Where(author => request.AuthorIds.Contains(author.Id))
            .ToListAsync();
        if (authors.Count != request.AuthorIds.Count)
        {
            return BookErrors.AuthorsNotFound();
        }

        var activeLoanCount = await dbContext.Loans
            .CountAsync(loan => loan.BookId == id && loan.ReturnedAt == null);
        if (request.TotalCopies < activeLoanCount)
        {
            return BookErrors.NotEnoughAvailableCopies(id);
        }

        bookMapper.UpdateEntity(request, book);
        book.Category = category;
        book.Authors = authors;
        book.AvailableCopies = request.TotalCopies - activeLoanCount;
        await dbContext.SaveChangesAsync();
        return bookMapper.ToReadDto(book);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
    {
        var book = await dbContext.Books.FirstOrDefaultAsync(book => book.Id == id);
        if (book is null)
        {
            return BookErrors.BookNotFound(id);
        }

        if (await dbContext.Loans.AnyAsync(loan => loan.BookId == id))
        {
            return BookErrors.HasLoans(id);
        }

        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync();
        return Result.Deleted;
    }
}
