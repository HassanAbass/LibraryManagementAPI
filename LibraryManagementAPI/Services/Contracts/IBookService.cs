using ErrorOr;
using LibraryManagementAPI.DTOs.Books;
using LibraryManagementAPI.RequestParameters;
using LibraryManagementAPI.RequestParameters.Enums;

namespace LibraryManagementAPI.Services.Contracts
{
    public interface IBookService
    {
        public Task<ErrorOr<ReadBookDto>> CreateBookAsync(CreateBookDto bookDto);
        Task<ErrorOr<List<ReadBookDto>>> Get(PaginationParameters pagination, BookFilteringParameters bookFilteringParameters, SortingParameters<BookSortBy> sortingParameters);
    }
}
