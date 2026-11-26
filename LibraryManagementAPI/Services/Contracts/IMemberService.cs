using ErrorOr;
using LibraryManagementAPI.DTOs.Members;

namespace LibraryManagementAPI.Services.Contracts
{
    public interface IMemberService
    {
        Task<ErrorOr<IEnumerable<ReadMemberDto>>> GetAllMembersAsync();
        Task<ErrorOr<ReadMemberDto>> GetByIdAsync(int id);
        Task<ErrorOr<ReadMemberDto>> CreateAsync(CreateMemberDto createMemberDto);
        Task<ErrorOr<ReadMemberDto>> UpdateAsync(int id, UpdateMemberDto updateMemberDto);
        Task<ErrorOr<Deleted>> DeleteAsync(int id);
    }
}