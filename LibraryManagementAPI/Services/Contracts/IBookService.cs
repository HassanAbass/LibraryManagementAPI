using ErrorOr;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.RequestParameters.Enums;

namespace LibraryManagementAPI.Services.Contracts;

public interface IBookService
{
    Task<ErrorOr<PagedResult<ReadBookDto>>> GetAsync(
        PaginationParameters pagination,
        BookFilteringParameters filtering,
        SortingParameters<BookSortBy> sorting);
    Task<ErrorOr<ReadBookDto>> GetByIdAsync(int id);
    Task<ErrorOr<ReadBookDto>> CreateAsync(CreateBookDto request);
    Task<ErrorOr<ReadBookDto>> UpdateAsync(int id, UpdateBookDto request);
    Task<ErrorOr<Deleted>> DeleteAsync(int id);
}
