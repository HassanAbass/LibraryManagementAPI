using ErrorOr;
using LibraryManagementAPI.DTOs.Authors;
using LibraryManagementAPI.RequestParameters;

namespace LibraryManagementAPI.Services.Contracts;

public interface IAuthorService
{
    Task<ErrorOr<List<ReadAuthorDto>>> GetAllAsync(PaginationParameters pagination);
    Task<ErrorOr<ReadAuthorDto>> GetByIdAsync(int id);
    Task<ErrorOr<ReadAuthorDto>> CreateAsync(CreateAuthorDto request);
    Task<ErrorOr<ReadAuthorDto>> UpdateAsync(int id, UpdateAuthorDto request);
    Task<ErrorOr<Deleted>> DeleteAsync(int id);
}
