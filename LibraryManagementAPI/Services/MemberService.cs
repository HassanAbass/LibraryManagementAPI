using ErrorOr;
using FluentValidation;
using LibraryManagementAPI.DataAccess;
using LibraryManagementAPI.DTOs.Members;
using LibraryManagementAPI.Errors;
using LibraryManagementAPI.Mapping;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class MemberService(
        LibraryDbContext context,
        MemberMapper memberMapper,
        IValidator<CreateMemberDto> createMemberValidator,
        IValidator<UpdateMemberDto> updateMemberValidator) : IMemberService
    {
        public async Task<ErrorOr<IEnumerable<ReadMemberDto>>> GetAllMembersAsync()
        {
            var members = await context.Members.ToListAsync();
            return members.Select(memberMapper.ToReadDto).ToList();
        }

        public async Task<ErrorOr<ReadMemberDto>> GetByIdAsync(int id)
        {
            var member = await context.Members.FirstOrDefaultAsync(member => member.Id == id);
            return member is null ? MembersError.NotFound(id) : memberMapper.ToReadDto(member);
        }

        public async Task<ErrorOr<ReadMemberDto>> CreateAsync(CreateMemberDto createMemberDto)
        {
            var validationResult = await createMemberValidator.ValidateAsync(createMemberDto);
            if (!validationResult.IsValid)
            {
                return validationResult.Errors
                    .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                    .ToList();
            }

            var member = memberMapper.ToEntity(createMemberDto);
            context.Members.Add(member);
            await context.SaveChangesAsync();
            return memberMapper.ToReadDto(member);
        }

        public async Task<ErrorOr<ReadMemberDto>> UpdateAsync(int id, UpdateMemberDto updateMemberDto)
        {
            var member = await context.Members.FirstOrDefaultAsync(member => member.Id == id);
            if (member is null)
            {
                return MembersError.NotFound(id);
            }

            var validationResult = await updateMemberValidator.ValidateAsync(updateMemberDto);
            if (!validationResult.IsValid)
            {
                return validationResult.Errors
                    .Select(error => Error.Validation(code: error.PropertyName, description: error.ErrorMessage))
                    .ToList();
            }

            memberMapper.UpdateEntity(updateMemberDto, member);
            await context.SaveChangesAsync();
            return memberMapper.ToReadDto(member);
        }

        public async Task<ErrorOr<Deleted>> DeleteAsync(int id)
        {
            var member = await context.Members.FirstOrDefaultAsync(member => member.Id == id);
            if (member is null)
            {
                return MembersError.NotFound(id);
            }
            member.IsActive = false;
            await context.SaveChangesAsync();
            return Result.Deleted;
        }
    }
}
